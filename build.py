﻿#!/bin/sh

#参数判断  
if [ $# != 4 ];then 
    echo "Error:参数个数不一致，打包失败"  
    exit     
fi 


#Param $1 projectPath /Users/hyz/JenkinsDemo/Main/TestJenkins
#Param $2 outputFolderPath /Users/hyz/Desktop
#Param $3 bundleIdentifier com.huanyz.g01
#Param $4 productName TestJenkins
#Param $5 bundleVersionCode 0
#Param $6 bundleVersion 1.0
#Param $7 gameName 测试名称
#Param $8 buildComment Jenkins自动打包流程
#  ./build.py "/Users/hyz/JenkinsDemo/Main/TestJenkins" "/Users/hyz/Desktop" "com.huanyz.g01" "TestJenkins" "0" "1.0" "Test" "JenkinsDemoPakage"


#/Users/hyz/TestJenkins/

#UNITY程序的路径#
UNITY_PATH=/Applications/Unity201702f3/Unity.app/Contents/MacOS/Unity

#游戏程序路径#
#PROJECT_PATH=$1
#打开unity工程#-batchmode
$UNITY_PATH -projectPath $1 -executeMethod BuildSteps.SetBuildConfig EnableDevelopBuild-$2 IsCustomPackName-$3 CustomPackName-$4 -logFile /Users/hyz/Desktop/IOSG01/G01PackIPA.txt -quit
#打出unity pack log
cat /Users/hyz/Desktop/IOSG01/G01PackIPA.txt
#echo "打开unity工程"
#设置unity参数#
#$UNITY_PATH -executeMethod QuickBuild.SetOutputFolderPath outputFolderPath-$2
#echo "设置输出路径成功:"$2
#$UNITY_PATH -executeMethod QuickBuild.SetBuildConfig bundleIdentifier-$3 productName-$4 bundleVersionCode-$5 bundleVersion-$6 gameName-$7 buildComment-$8
#echo "设置BundleConfig成功:"$3"|"$4"|"$5"|"$6"|"$7"|"$8
echo "生成xcode工程完毕"
echo "生成ipa完成"
#打开生成的ipa文件夹#
#$UNITY_PATH -executeMethod QuickBuild.OpenLastBuildFolder
#关闭Unity#
#$UNITY_PATH -quit
