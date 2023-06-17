package main

import (
	"os"
)

type ConfigItem struct {
	Name      string
	Command   string
	Dest      string
	FileBlock FileBlockOffset
}

type FileBlockOffset struct {
	Offset uint64
	Size   uint64
}

func OpenSelf() *os.File {
	ret, err := os.Open(os.Args[0])
	if err != nil {
		return nil
	}

	return ret
}

func ReadConfig(file *os.File) []ConfigItem {
	count, err := readUint32(file)
	if err != nil {
		return nil
	}

	var ret = make([]ConfigItem, count)

	for i := uint32(0); i < count; i++ {
		tmp, err := readString(file)
		if err != nil {
			return nil
		}

		ret[i].Name = tmp

		tmp, err = readString(file)

		if err != nil {
			return nil
		}

		ret[i].Command = tmp

		tmp, err = readString(file)

		if err != nil {
			return nil
		}

		ret[i].Dest = tmp

		size, err := readUint64(file)
		if err != nil {
			return nil
		}

		offset, err := file.Seek(0, 1)
		if err != nil {
			return nil
		}

		ret[i].FileBlock.Offset = uint64(offset)
		ret[i].FileBlock.Size = size

		// skip file content
		file.Seek(int64(size), 1)

		// log.Default().Println("Found ", ret[i].Name, " command: ", ret[i].Command, " dest: ", ret[i].Dest)
	}

	return ret
}
