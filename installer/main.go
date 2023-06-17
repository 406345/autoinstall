package main

import (
	"fmt"
	"log"
	"os"
)

var EXE_SIZE uint64 = 4895232

var sshHost string = "127.0.0.1"
var sshAccount string = ""
var sshPassword string = ""

func findPESize(self *os.File, offset int64) int32 {
	self.Seek(offset, 0)
	peSize, _ := readUint32(self)
	self.Seek(0, 0)
	return int32(peSize)
}

func main() {
	// test()

	var exe_path = os.Args[0]

	var file, err = os.Open(exe_path)
	if err != nil {
		log.Default().Fatalln("open selft error: ", err)
		return
	}

	info, _ := os.Lstat(exe_path)
	EXE_SIZE = uint64(findPESize(file, info.Size()-4))
	file.Seek(int64(EXE_SIZE), 0)

	// Read config file
	configs := ReadConfig(file)
	log.Default().Println("Config count: ", len(configs))

	log.Default().Print("Enter SSH server: ")
	fmt.Scanf("%s\n", &sshHost)
	log.Default().Print("Enter SSH account: ")
	fmt.Scanf("%s\n", &sshAccount)
	log.Default().Print("Enter SSH password: ")
	fmt.Scanf("%s\n", &sshPassword)

	for i := 0; i < len(configs); i++ {
		config := &configs[i]
		err := RunConfig(config)
		if err != nil {
			log.Default().Printf("Error %v", err)
			return
		}
	}
}

func RunConfig(config *ConfigItem) error {
	client, err := SSHLogin(sshHost, sshAccount, sshPassword)
	if err != nil {
		log.Default().Println("Failed to create ssh connection")
		return err
	}
	defer client.Close()

	log.Default().Printf("Processing %s...", config.Name)

	err = SSHUploadFile(client, config, nil)
	if err != nil {
		log.Default().Printf("Upload file %s failed: %v", config.Name, err)
	}

	err = SSHCommand(client, config, func(s string) {
		log.Default().Printf("%s\n%s", config.Command, s)
	})

	if err != nil {
		log.Default().Printf("Execute %s failed: %v", config.Command, err)
		return err
	}

	return nil
}
