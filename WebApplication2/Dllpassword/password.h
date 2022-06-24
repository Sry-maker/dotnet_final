#pragma once
#define _CRT_SECURE_NO_WARNINGS
#include <stdio.h>
#include <malloc.h>
#include <string.h>
#define NUM 20
#define _ ios_base::sync_with_stdio(0),cin.tie(0)
char KEY[26] = { 'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z' };
char PWD[26] = { 'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z' };

extern "C" __declspec(dllexport) char* Encryption(char strtest[NUM]);
extern "C" __declspec(dllexport) char* Decryption(char strtest[NUM]);