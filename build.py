#!/bin/sh

#参数判断  
if [ $# != 8 ];then 
    echo "需要一个参数。 参数是游戏包的名子"  
    exit     
fi  

#Param $1 projectPath
#Param $2 outputFolderPath
#Param $3 bundleIdentifier
#Param $4 productName
#Param $5 bundleVersionCode
#Param $6 bundleVersion
#Param $7 gameName
#Param $8 buildComment

#/Users/hyz/TestJenkins/

#UNITY程序的路径#
UNITY_PATH=/Applications/Unity201702f3/Unity.app/Contents/MacOS/Unity

#游戏程序路径#
PROJECT_PATH=$1
#打开unity工程#
$UNITY_PATH -projectPath $PROJECT_PATH 
echo "打开unity工程"
#设置unity参数#
$UNITY_PATH -executeMethod QuickBuild.SetOutputFolderPath outputFolderPath-$1
echo "设置输出路径成功:"$2
$UNITY_PATH -executeMethod QuickBuild.SetBuildConfig bundleIdentifier-$3 productName-$4 bundleVersionCode-$5 bundleVersion-$6 gameName-$7 buildComment-$8
echo "设置BundleConfig成功:"$3"|"$4"|"$5"|"$6"|"$7"|"$8
echo "生成xcode工程完毕"
echo "生成ipa完成"
#打开生成的ipa文件夹#
$UNITY_PATH -executeMethod QuickBuild.OpenLastBuildFolder
#关闭Unity#
$UNITY_PATH -quit
