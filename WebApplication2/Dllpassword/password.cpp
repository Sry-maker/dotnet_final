#include "password.h"
char* Encryption(char strtest[NUM])
{
	char* str = (char*)malloc(NUM * sizeof(char));
	for (int i = 0; i < 20; i++) {
		str[i] = '\0';

	}
	strcpy(str, strtest);
	int i = 0;
	for (i = 0; i < NUM; i++) {
		if (strtest[i] == '/0') {
			break;
		}

	}
	int n = i + 1;
	int index;
	for (int i = 0; i < n; i++)
	{
		index = (int)str[i];
		if (str[i] >= 'a' && str[i] <= 'z')
		{
			index -= (int)'a';
			index += 3;
			if (index >= 26)
				index -= 26;
			str[i] = PWD[index];
		}
		else if (str[i] >= 'A' && str[i] <= 'Z')
		{
			index -= (int)'A';
			index -= 2;
			if (index < 0)
				index += 26;
			str[i] = KEY[index];
		}
	}
	//for (i = 0; i < NUM; i++) {
	//	if (str[i] == '/0') {
	//		break;
	//	}
	//	else {
	//		printf("%c", str[i]);
	//	}

	//}
	return str;
}
//½âÃÜ 
char* Decryption(char strtest[NUM])
{
	char* str = (char*)malloc(NUM * sizeof(char));
	for (int i = 0; i < 20; i++) {
		str[i] = '\0';

	}
	strcpy(str, strtest);
	int i = 0;
	for (i = 0; i < NUM; i++) {
		if (strtest[i] == '/0') {
			break;
		}

	}
	int n = i + 1;
	int index;
	for (int i = 0; i < n; i++)
	{
		index = (int)str[i];
		if (str[i] >= 'A' && str[i] <= 'Z')
		{
			index -= (int)'A';
			index -= 3;
			if (index < 0)
				index += 26;
			str[i] = KEY[index];
		}
		else if (str[i] >= 'a' && str[i] <= 'z')
		{
			index -= (int)'a';
			index += 2;
			if (index >= 26)
				index -= 26;
			str[i] = PWD[index];
		}
	}
	return str;
}
