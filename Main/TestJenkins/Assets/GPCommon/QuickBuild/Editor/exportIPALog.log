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
xcodebuild archive -scheme "Unity-iPhone" -configuration "Release" -archivePath ${archiveName}
xcodebuild -exportArchive -archivePath ${archiveName} -exportPath ${name} -exportOptionsPlist Info.plist

