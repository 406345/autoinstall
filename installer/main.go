package main

import (
	"fmt"
	"log"
	"os"

	"github.com/nsf/termbox-go"
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
	err := termbox.Init()
	if err != nil {
		return
	}

	err = Deploy()
	if err != nil {
		log.Default().Printf("Error: %v", err)
	} else {
		log.Default().Print("All done")
	}

	tmp := ""
	fmt.Scanf("%s\n", &tmp)
}

func ReadInput(mask bool) (string, error) {
	ret := ""

	lastLen := 0
	for {
		ev := termbox.PollEvent()
		if ev.Type == termbox.EventKey {
			lastLen = len(ret)
			if ev.Key == termbox.KeyEnter {
				if mask {
					for i := 0; i < len(ret); i++ {
						if mask {
							fmt.Print("*")
						} else {
							fmt.Print(string(rune(ret[i])))
						}
					}

				} else {
					fmt.Print(ret)
				}
				fmt.Print("\n")
				return ret, nil
			} else if ev.Key == termbox.KeyBackspace && len(ret) > 0 {
				ret = ret[:len(ret)-1]
			} else {
				ret += string(ev.Ch)
			}

			for i := 0; i < len(ret); i++ {
				if mask {
					fmt.Print("*")
				} else {
					fmt.Print(string(rune(ret[i])))
				}
			}

			for i := 0; i < lastLen-len(ret); i++ {
				fmt.Print(" ")
			}

			fmt.Print("\r")
			//log.Default().Printf("%v %v", ret, ev.Ch)
		}
	}

	return "", nil
}

func Deploy() error {

	defer termbox.Close()

	var exe_path = os.Args[0]

	var file, err = os.Open(exe_path)
	if err != nil {
		log.Default().Fatalln("open selft error: ", err)
		return err
	}

	info, _ := os.Lstat(exe_path)
	EXE_SIZE = uint64(findPESize(file, info.Size()-4))
	file.Seek(int64(EXE_SIZE), 0)

	// Read config file
	configs := ReadConfig(file)
	log.Default().Println("Config count: ", len(configs))

	log.Default().Print("Enter SSH server: ")
	sshHost, _ = ReadInput(false)
	log.Default().Print("Enter SSH account: ")
	sshAccount, _ = ReadInput(false)
	log.Default().Print("Enter SSH password: ")
	sshPassword, _ = ReadInput(true)

	for i := 0; i < len(configs); i++ {
		config := &configs[i]
		err := RunConfig(config)
		if err != nil {
			return err
		}
	}

	return nil
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
		return err
	}

	return nil
}
