package main

import (
	"errors"
	"fmt"
	"io"
	"log"
	"path/filepath"
	"strings"
	"time"

	"golang.org/x/crypto/ssh"
)

const PRGORESS_SIMBOL_SIZE = 20

func SSHLogin(addr string, account string, password string) (*ssh.Client, error) {

	if !strings.Contains(addr, ":") {
		addr += ":22"
	}

	var config = &ssh.ClientConfig{
		User:            account,
		Auth:            []ssh.AuthMethod{ssh.Password(password)},
		HostKeyCallback: ssh.InsecureIgnoreHostKey(),
		Timeout:         10 * time.Second,
	}

	client, err := ssh.Dial("tcp", addr, config)

	if err != nil {
		return nil, err
	}

	return client, nil
}

func SSHCommand(client *ssh.Client, item *ConfigItem, output func(string)) error {

	if client == nil {
		return errors.New("ssh client is null")
	}

	if item == nil {
		return errors.New("config is null")
	}

	dir := "/"
	if item.Dest != "" {
		dir = filepath.Dir(item.Dest)
		dir = strings.Replace(dir, "\\", "/", -1)
	}

	if len(item.Command) == 0 {
		return nil
	}

	for i := 0; i < len(item.Command); i++ {
		sess, err := client.NewSession()

		if err != nil {
			return err
		}
		defer sess.Close()

		cmd := item.Command[i]

		log.Default().Printf(" > %v", cmd)
		ret, err := sess.CombinedOutput(fmt.Sprintf("cd %v;%v", dir, cmd))
		if err != nil {
			return err
		}

		if output != nil {
			output(string(ret))
		}
	}
	return nil
}

func SSHUploadFile(client *ssh.Client, item *ConfigItem, progress func(int32)) error {

	if item.Dest == "" {
		return nil
	}

	dir := filepath.Dir(item.Dest)
	dir = strings.Replace(dir, "\\", "/", -1)

	scp, err := client.NewSession()
	if err != nil {
		return err
	}
	defer scp.Close()

	sign := make(chan error, 1)
	defer close(sign)

	file := OpenSelf()
	if file == nil {
		return errors.New("error open self")
	}
	defer file.Close()

	w, err := scp.StdinPipe()
	if err != nil {
		return err
	}

	fmt.Printf("Upload files to %v\n", item.Dest)

	go func() {
		defer w.Close()
		fmt.Fprintln(w, "C0644", item.FileBlock.Size, filepath.Base(item.Dest))
		file.Seek(int64(item.FileBlock.Offset), 0)

		blockSize := 1024 * 1024
		var totalSize int64 = int64(item.FileBlock.Size)
		var count int64 = 0
		buffer := make([]byte, blockSize)

		for {
			reads, err := file.Read(buffer)
			if totalSize <= 0 || errors.Is(err, io.EOF) {
				break
			}

			if err != nil {
				log.Default().Printf("read error: %v", err)
				sign <- err
				return
			}

			if totalSize < int64(reads) {
				reads = int(totalSize)
				totalSize = 0
			} else {
				totalSize -= int64(reads)
			}

			count += int64(reads)
			w.Write(buffer[:reads])

			percent := (uint64(count) * 100 / item.FileBlock.Size)
			fmt.Printf("Progress: ")

			finished := int(float32(percent) / float32(100) * PRGORESS_SIMBOL_SIZE)
			for i := 0; i < finished; i++ {
				fmt.Printf("+")
			}

			for i := 0; i < PRGORESS_SIMBOL_SIZE-finished; i++ {
				fmt.Printf("-")
			}
			fmt.Printf(" (%2d%%)", percent)

			fmt.Print("\r")

			if progress != nil {
				v := count * int64(100) / int64(item.FileBlock.Size)
				progress(int32(v))
			}
		}

		if progress != nil {
			progress(int32(100))
		}

		fmt.Print("\n")

		fmt.Fprint(w, "\x00")
		sign <- io.EOF
	}()

	scp.Run("/usr/bin/scp -ptr '" + dir + "'")

	erroper := <-sign

	if !errors.Is(erroper, io.EOF) {
		log.Default().Printf("Upload error: %v", erroper)
		return erroper
	}

	return nil
}
