## 关于

这是一个可以把多个文件，脚本，资源等合并并且生成一个可执行EXE的小程序。并且自带SSH和SCP功能，支持Windows到LInux的远程拷贝，执行命令；方便部署。

## 运行方法

```
generator.exe /path/to/the/configfile.txt
```

运行后会生成一个autoinstall.exe可执行程序

## 配置文件DEMO

```
[
    {
        "name" : "main",
        "command" : "unzip -o /tmp/test/main.zip",
        "file" : "D:/project/autoinstall/main.zip",
        "remote":"/tmp/test/main.zip"
    },
    {
        "name" : "generator",
        "command" : "unzip -o /tmp/test/generator.zip",
        "file" : "D:/project/autoinstall/generator.zip",
        "remote":"/tmp/test/generator.zip"
    },
    
    {
        "name" : "ll",
        "command" : "ls -l;pwd", 
	    "remote" : "/var/"
    }
]
```

name: 必填项不能重复

command: 需要执行的远程命令

file: 需要打包的本地文件

remote: 需要把file拷贝到远程LINUX服务器的路径

### 注意事项

- name 必填
- 如果配置了file，则必须配置remote
- command可以选填,command+remote则使用remote作为执行目录
- 执行顺序是 首先拷贝file到remote（如果配置），然后执行command

