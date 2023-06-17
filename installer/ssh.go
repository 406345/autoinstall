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

	dir := filepath.Dir(item.Dest)
	dir = strings.Replace(dir, "\\", "/", -1)

	if item.Command == "" {
		return nil
	}

	sess, err := client.NewSession()
	if err != nil {
		return err
	}
	defer sess.Close()

	ret, err := sess.CombinedOutput("cd " + dir + ";" + item.Command)
	if err != nil {
		return err
	}

	if output != nil {
		output(string(ret))
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

			log.Default().Printf("Upload %v/%v(%v%%)", count, item.FileBlock.Size, (uint64(count) * 100 / item.FileBlock.Size))

			if progress != nil {
				v := count * int64(100) / int64(item.FileBlock.Size)
				progress(int32(v))
			}
		}

		if progress != nil {
			progress(int32(100))
		}

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
