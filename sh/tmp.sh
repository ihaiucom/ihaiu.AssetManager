#!/usr/bin/expect  -f   


cd /Users/zengfeng/workspaces/www/githubpages/ihaiu.AssetManager/ihaiu.AssetManager/Assets/../../../test.version.git/

spawn git push origin osx
expect "Password:"
send "git\r"
interact

spawn git push origin  :refs/tags/osx_ver1.0.1_official
expect "Password:"
send "git\r"
interact

spawn git push origin  :refs/tags/osx_ver1.0.1_xiaomi
expect "Password:"
send "git\r"
interact

spawn git push origin  :refs/tags/osx_ver1.0.1_weixin
expect "Password:"
send "git\r"
interact

spawn git push origin  :refs/tags/osx_ver1.0.1_uc
expect "Password:"
send "git\r"
interact

spawn git push origin --tags
expect "Password:"
send "git\r"
interact
