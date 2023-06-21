package main

import (
	"encoding/binary"
	"errors"
	"os"
)

func readBytes(file *os.File) ([]byte, error) {
	len, err := readUint32(file)

	if err != nil {
		return nil, errors.New("read string failed")
	}

	if len == 0 {
		return nil, nil
	}

	var content = make([]byte, len)
	reads, err := file.Read(content)
	// file.Seek(int64(len), 1)
	if err != nil || reads == 0 {
		return nil, errors.New("read content failed")
	}

	return content, nil
}

func readString(file *os.File) (string, error) {
	len, err := readUint16(file)

	if err != nil {
		return "", errors.New("read string failed")
	}

	if len == 0 {
		return "", nil
	}

	var content = make([]byte, len)
	reads, err := file.Read(content)
	// file.Seek(int64(len), 1)
	if err != nil || reads == 0 {
		return "", errors.New("read content failed")
	}

	return string(content), nil
}

func readUint16(file *os.File) (uint16, error) {
	var length [2]byte
	reads, err := file.Read(length[:])
	// file.Seek(2, 1)
	if err != nil || reads == 0 {
		return 0, errors.New("read string failed")
	}

	var ret = binary.LittleEndian.Uint16(length[:])
	return ret, nil
}

func readUint32(file *os.File) (uint32, error) {
	var length [4]byte
	reads, err := file.Read(length[:])
	// file.Seek(4, 1)
	if err != nil || reads == 0 {
		return 0, errors.New("read string failed")
	}

	var ret = binary.LittleEndian.Uint32(length[:])
	return ret, nil
}

func readUint64(file *os.File) (uint64, error) {
	var length [8]byte
	reads, err := file.Read(length[:])
	// file.Seek(8, 1)
	if err != nil || reads == 0 {
		return 0, errors.New("read string failed")
	}

	var ret = binary.LittleEndian.Uint64(length[:])
	return ret, nil
}
