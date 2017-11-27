#!/bin/sh
path=${1}
name=${2}
ipaname=${3}

archiveName=${name}.xcarchive

echo exportIPA Log here!
echo ${path}
echo ${name}
echo ${ipaname}
echo ${archiveName}

cd ${path}

security set-keychain-settings -t 3600 -l ~/Library/Keychains/login.keychain-db
security unlock-keychain -p "123456" ~/Library/Keychains/login.keychain-db

xcodebuild clean
xcodebuild archive -scheme "Unity-iPhone" -configuration "Release" -archivePath ${archiveName}
xcodebuild -exportArchive -archivePath ${archiveName} -exportPath ${name} -exportOptionsPlist Info.plist
#rename
mv ${name}/Unity-iPhone.ipa ${name}/${ipaname}.ipa
echo ${path}
echo "rename Succ"
#copy到websever
\cp ${name}/${ipaname}.ipa /Users/hyz/Documents/WebServer/ipa
echo "cp Succ"

