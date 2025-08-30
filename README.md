MorkReader: A C# implementation of a Mork Parser
---

This project is the conversion from Java to C# of this source code: https://github.com/ibauersachs/jmork

MorkReader is starting to work... but it's not finished yet!

There is also a concise Python version here: https://github.com/ibauersachs/jmork/blob/master/doc/demork.py

The current Thunderbird source code is in C++, the Github mirror copy is here: https://github.com/mozilla/releases-comm-central/tree/master/mailnews/db/mork  
and the main source is here:  
https://hg-edge.mozilla.org/comm-central/file/tip/mailnews/db/mork

See also:  
https://wiki.mozilla.org/Mork  
https://github.com/KevinGoodsell/mork-converter/blob/master/doc/mork-format.txt  
https://github.com/KevinGoodsell/mork-converter  (in Python)

How to export a Thunderbird Index.msf file to Xml?
- Clone online this repository: https://github.com/KevinGoodsell/mork-converter (there is also [this one](https://github.com/crass/mork-converter/tree/python3) in Python 3, but not tested)
- Clone locally your cloned repository, for exemple: C:\Github\mork-converter 
- Install [Python 2.7.18: www.python.org/downloads/release/python-2718](https://www.python.org/downloads/release/python-2718)
- Install PLY (Python Lex-Yacc) in a DOS prompt:
```
cd C:\Python27\Scripts
pip install ply
```
- Check that PLY is installed
```
C:\Python27\python.exe -c "import ply; print(ply.__version__)"
--> 3.11
```
- Run C:\Github\mork-converter\setup.py in a DOS prompt:
```
cd C:\Github\mork-converter
C:\Python27\python.exe setup build
C:\Python27\python.exe setup install
```
- Copy your Inbox.msf file into Mork directory: C:\Github\mork-converter\src
- Run C:\Github\mork-converter\src\Mork in a DOS prompt:
```
cd C:\Github\mork-converter\src
C:\Python27\python.exe mork --outname=Inbox.xml Inbox.msf
```