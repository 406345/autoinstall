## ����

����һ�����԰Ѷ���ļ����ű�����Դ�Ⱥϲ���������һ����ִ��EXE��С���򡣲����Դ�SSH��SCP���ܣ�֧��Windows��LInux��Զ�̿�����ִ��������㲿��

## ���з���

```
generator.exe /path/to/the/configfile.txt
```

���к������һ��autoinstall.exe��ִ�г���

## �����ļ�DEMO

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

name: ��������ظ�

command: ��Ҫִ�е�Զ������

file: ��Ҫ����ı����ļ�

remote: ��Ҫ��file������Զ��LINUX��������·��

### ע������

- name ����
- ���������file�����������remote
- command����ѡ��,command+remote��ʹ��remote��Ϊִ��Ŀ¼
- ִ��˳���� ���ȿ���file��remote��������ã���Ȼ��ִ��command

