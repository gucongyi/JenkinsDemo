#!/bin/sh
path=${1}
name=${2}
archiveName=${name}.xcarchive

echo exportIPA Log here!
echo ${path}
echo ${name}
echo ${archiveName}

cd ${path}
xcodebuild clean
security set-keychain-settings -t 3600 -l ~/Library/Keychains/login.keychain-db
security unlock-keychain -p "123456" ~/Library/Keychains/login.keychain-db
xcodebuild archive -scheme "Unity-iPhone" -configuration "Release" -archivePath ${archiveName}
xcodebuild -exportArchive -archivePath ${archiveName} -exportPath ${name} -exportOptionsPlist Info.plist

