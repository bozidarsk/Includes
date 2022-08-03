#ifndef CHARSTRING_H

#define CHARSTRING_H

#ifndef STRING_H
#define EMPTYSTR ""
#define EMPTYCHAR '\0'

#ifndef _CTYPE_H
#if __has_include("charstring.h")
#include <ctype.h>
#endif
#endif
#endif

#ifndef STRING_H
int slen(const char _main[]) 
{
	int i = 0;
	while (_main[i] != '\0') { i++; }
	return i;
}
#endif

bool IsNullOrEmpty(const char str[]) { return (slen(str) <= 0 || str[0] == EMPTYCHAR) ? true : false; }
bool Equal(const char str1[], const char str2[]) 
{
	if (slen(str1) != slen(str2)) { return false; }
	for (int i = 0; i < slen(str1); i++) { if (str1[i] != str2[i]) { return false; } }
	return true;
}

#ifndef STRING_H
char* ToString(const char _main) 
{
	if (_main == EMPTYCHAR) { return (char*)EMPTYSTR; }
	char* output = (char*)malloc(1);
	output[0] = _main;
	return output;
}
#endif

char* Copy(const char _main[]) 
{
	int len = slen(_main);
	char* main = (char*)malloc(len);
	for (int i = 0; i < len; i++) { main[i] = _main[i]; }
	return main;
}

char* Join(const char str1[], const char str2[]) 
{
	bool b1 = IsNullOrEmpty(str1);
	bool b2 = IsNullOrEmpty(str2);
	if (b1 && b2) { return (char*)EMPTYSTR; }
	if (b1 && !b2) { return (char*)str2; }
	if (!b1 && b2) { return (char*)str1; }

	int t = 0;
	char* main = (char*)malloc(slen(str1) + slen(str2));
	for (int i = 0; i < slen(str1); i++) { main[t] = str1[i]; t++; }
	for (int i = 0; i < slen(str2); i++) { main[t] = str2[i]; t++; }

	return main;
}

char* Remove(const char _main[], const int startIndex) 
{
	if (IsNullOrEmpty(_main) || startIndex < 0 || startIndex >= slen(_main)) { return (char*)_main; }

	int i = 0;
	char* newString = (char*)malloc(startIndex);
	while (i < startIndex) { newString[i] = _main[i]; i++; }
	return newString;
}

char* Remove(const char _main[], const int startIndex, const int count) 
{
	if (IsNullOrEmpty(_main) || startIndex + count > slen(_main) || count <= 0 || count > slen(_main) || startIndex < 0 || startIndex >= slen(_main)) { return (char*)_main; }

	int i = 0;
	int s = 0;
	char* newString = (char*)malloc(slen(_main) - count);
	while (i < slen(_main)) 
	{
		if (i == startIndex) { i += count; }
		newString[s] = _main[i];
		s++;
		i++;
	}

	return newString;
}

char* Reverse(const char _main[]) 
{
    if (IsNullOrEmpty(_main)) { return (char*)_main; }
    char* main = Copy(_main);

    int x = 0;
    int y = slen(main) - 1;
    char rightChar = '0';
    while (x < slen(main) / 2) 
    {
    	rightChar = main[y];
    	main[y] = main[x];
    	main[x] = rightChar;
    	x++;
    	y--;
    }

    return main;
}

char* GetStringAt(const char _main[], const int startPos, const int endPos) 
{
    if (IsNullOrEmpty(_main) || startPos < 0 || endPos <= 0 || endPos < startPos || endPos >= slen(_main)) { return (char*)EMPTYSTR; }

    char* output = (char*)malloc((endPos - startPos) + 1);
    int i = startPos;
    int t = 0;

    while (t < (endPos - startPos) + 1) 
    {
        output[t] = _main[i];
        i++;
        t++;
    }

    return output;
}

bool StartsWith(const char _main[], const char _target[]) 
{
	if (IsNullOrEmpty(_main) || IsNullOrEmpty(_target) || slen(_target) > slen(_main)) { return false; }

    int i = 0;
    bool pass = true;

    if (slen(_main) < slen(_target)) { pass = false; }
    while (i < slen(_target) && pass) 
    {
        if (_main[i] != _target[i]) { pass = false; }
        i++;
    }

    return pass;
}

bool EndsWith(const char _main[], const char _target[]) 
{
	if (IsNullOrEmpty(_main) || IsNullOrEmpty(_target) || slen(_target) > slen(_main)) { return false; }

    int i = slen(_main) - 1;
    int t = slen(_target) - 1;
    bool pass = true;

    if (slen(_main) < slen(_target)) { pass = false; }
    while (i >= slen(_main) - slen(_target) && pass) 
    {
        if (_main[i] != _target[t]) { pass = false; }
        i--;
        t--;
    }

    return pass;
}

int IndexOf(const char _main[], const char _target[]) 
{
	if (IsNullOrEmpty(_main) || IsNullOrEmpty(_target) || slen(_target) > slen(_main)) { return -1; }

	int i = 0;
	int t = 0;
	while (i < slen(_main)) 
	{
		t = 0;
		while (t < slen(_target)) 
		{
			if (_main[i + t] != _target[t]) { break; }
			t++;
		}

		if (t >= slen(_target)) { return i; }
		i++;
	}

	return -1;
}

int IndexOf(const char _main[], const char _target[], const int startIndex) 
{
	if (IsNullOrEmpty(_main) || IsNullOrEmpty(_target) || slen(_target) > slen(_main) || startIndex < 0 || startIndex >= slen(_main)) { return -1; }

	int i = startIndex;
	int t = 0;
	while (i < slen(_main)) 
	{
		t = 0;
		while (t < slen(_target)) 
		{
			if (_main[i + t] != _target[t]) { break; }
			t++;
		}

		if (t >= slen(_target)) { return i; }
		i++;
	}

	return -1;
}

int LastIndexOf(const char _main[], const char _target[]) 
{
	if (IsNullOrEmpty(_main) || IsNullOrEmpty(_target) || slen(_target) > slen(_main)) { return -1; }

	int i = slen(_main) - slen(_target);
	int t = 0;
	while (i >= 0) 
	{
		t = 0;
		while (t < slen(_target)) 
		{
			if (_main[i + t] != _target[t]) { break; }
			t++;
		}

		if (t >= slen(_target)) { return i; }
		i--;
	}

	return -1;
}

bool Contains(const char _main[], const char _target[]) 
{
	if (IsNullOrEmpty(_main) || IsNullOrEmpty(_target) || slen(_target) > slen(_main)) { return false; }

	int i = 0;
	int t = 0;
	while (i < slen(_main)) 
	{
		t = 0;
		while (t < slen(_target)) 
		{
			if (_main[i + t] != _target[t]) { break; }
			t++;
		}

		if (t >= slen(_target)) { return true; }
		i++;
	}

	return false;
}

int Count(const char _main[], const char _target) 
{
	if (IsNullOrEmpty(_main) || _target == EMPTYCHAR) { return 0; }

	int count = 0;
	for (int i = 0; i < slen(_main); i++) { count += (_main[i] == _target) ? 1 : 0; }
	return count;
}

int Count(const char _main[], const char _target[]) 
{
	if (IsNullOrEmpty(_main) || IsNullOrEmpty(_target) || slen(_target) > slen(_main)) { return 0; }

	int i = IndexOf(_main, _target);
	int count = 0;

	while (i >= 0) 
	{
		i = IndexOf(_main, _target, i + 1);
		count++;
	}

	return count;
}

char* Insert(const char _main[], const char _target[], const int index) 
{
	if (IsNullOrEmpty(_main) || IsNullOrEmpty(_target) || index < 0 || index >= slen(_main)) { return (char*)_main; }
	int mlen = slen(_main) + slen(_target);
	char* main = (char*)malloc(mlen);

	int i = 0;
	int m = 0;
	int t = 0;
	while (i < mlen) 
	{
		if (i >= index && i < index + slen(_target)) { main[i] = _target[t]; t++; }
		else { main[i] = _main[m]; m++;; }
		i++;
	}

	return main;
}

char* Replace(const char _main[], const char _from[], const char _to[]) 
{
	if (IsNullOrEmpty(_main) || IsNullOrEmpty(_from) || IsNullOrEmpty(_to) || slen(_from) > slen(_main)) { return (char*)_main; }

	int i = IndexOf(_main, _from);
	if (i < 0) { return (char*) _main; }

	char* removed = Remove(_main, i, slen(_from));
	char* inserted = Insert(removed, _to, i);

	return (Contains(inserted, _from)) ? Replace(inserted, _from, _to) : inserted;
}

char* Replace(const char _main[], const char _from[], const char _to[], const int count) 
{
	if (count <= 0 || IsNullOrEmpty(_main) || IsNullOrEmpty(_from) || IsNullOrEmpty(_to) || slen(_from) > slen(_main)) { return (char*)_main; }

	int i = IndexOf(_main, _from);
	if (i < 0) { return (char*) _main; }

	char* removed = Remove(_main, i, slen(_from));
	char* inserted = Insert(removed, _to, i);

	return (Contains(inserted, _from)) ? Replace(inserted, _from, _to, count - 1) : inserted;
}

char* FillString(const char _main[], const char _target) 
{
	if (IsNullOrEmpty(_main) || _target == EMPTYCHAR) { return (char*)_main; }

	char* output = Copy(_main);
	for (int i = 0; i < slen(_main); i++) { output[i] = _target; }
	return output;
}

#ifndef STRING_H
char* FillString(const char _target, const int count) 
{
	if (_target == EMPTYCHAR || count <= 0) { return (char*)EMPTYSTR; }

	char* output = (char*)malloc(count);
	for (int i = 0; i < count; i++) { output[i] = _target; }
	return output;
}
#endif

char* TrimStart(const char _main[], const int count) 
{
	if (IsNullOrEmpty(_main) || count <= 0 || count > slen(_main)) { return (char*)_main; }

	char* output = (char*)malloc(slen(_main) - count);
	for (int i = 0; i < slen(_main) - count; i++) { output[i] = _main[i + count]; }
	return output;
}

char* TrimStart(const char _main[], const char _target) 
{
	if (IsNullOrEmpty(_main) || _target == EMPTYCHAR || !StartsWith(_main, ToString(_target))) { return (char*)_main; }

	int i = 0;
	while (i < slen(_main)) { if (_main[i] != _target) { break; } i++; }
	return TrimStart(_main, i - 1);
}

char* TrimEnd(const char _main[], const int count) 
{
	if (IsNullOrEmpty(_main) || count <= 0 || count > slen(_main)) { return (char*)_main; }

	char* output = (char*)malloc(slen(_main) - count);
	for (int i = 0; i < slen(_main) - count; i++) { output[i] = _main[i]; }
	return output;
}

char* TrimEnd(const char _main[], const char _target) 
{
	if (IsNullOrEmpty(_main) || _target == EMPTYCHAR || !EndsWith(_main, ToString(_target))) { return (char*)_main; }

	int i = slen(_main) - 1;
	while (i >= 0) { if (_main[i] != _target) { break; } i--; }
	return TrimEnd(_main, slen(_main) - i - 1);
}

#ifndef STRING_H
char* ToString(const float _main) 
{
	int digits = 0;
	float result = _main;
	while (result / 10 > 0) { result /= 10; digits++; }

	char* output = (char*)malloc(digits);
	sprintf(output, "%f", _main);
	return TrimEnd(output, '0');
}

char* ToString(const double _main) 
{
	int digits = 0;
	double result = _main;
	while (result / 10 > 0) { result /= 10; digits++; }

	char* output = (char*)malloc(digits);
	sprintf(output, "%f", _main);
	return TrimEnd(output, '0');
}

char* ToString(const int _main) 
{
	int digits = 0;
	int result = _main;
	while (result % 10 != 0) { result /= 10; digits++; }

	char* output = (char*)malloc(digits);
	sprintf(output, "%i", _main);
	return output;
}
#endif

char* ToUpper(const char _main[]) 
{
	if (IsNullOrEmpty(_main)) { return (char*)_main; }

	char* output = Copy(_main);
	for (int i = 0; i < slen(_main); i++) 
	{
		output[i] = (char)toupper(_main[i]);
	}

	return output;
}

char* ToLower(const char _main[]) 
{
	if (IsNullOrEmpty(_main)) { return (char*)_main; }
	
	char* output = Copy(_main);
	for (int i = 0; i < slen(_main); i++) 
	{
		output[i] = (char)tolower(_main[i]);
	}

	return output;
}

#ifndef STRING_H
char ToUpper(const char _main) { return (char)toupper(_main); }
char ToLower(const char _main) { return (char)tolower(_main); }
bool IsUpper(const char _main) { return isupper(_main); }
bool IsLower(const char _main) { return islower(_main); }
bool IsDigit(const char _main) { return isdigit(_main); }
bool IsAlphabetic(const char _main) { return isalpha(_main); }
bool IsPrintable(const char _main) { return isprint(_main); }
bool IsControl(const char _main) { return iscntrl(_main); }
#endif

bool IsUpper(const char _main[]) 
{
	for (int i = 0; i < slen(_main); i++) { if (!isupper(_main[i])) { return false; } }
	return true;
}

bool IsLower(const char _main[]) 
{
	for (int i = 0; i < slen(_main); i++) { if (!islower(_main[i])) { return false; } }
	return true;
}

bool IsDigit(const char _main[]) 
{
	for (int i = 0; i < slen(_main); i++) { if (!isdigit(_main[i])) { return false; } }
	return true;
}

bool IsAlphabetic(const char _main[]) 
{
	for (int i = 0; i < slen(_main); i++) { if (!isalpha(_main[i])) { return false; } }
	return true;
}

bool IsPrintable(const char _main[]) 
{
	for (int i = 0; i < slen(_main); i++) { if (!isprint(_main[i])) { return false; } }
	return true;
}

bool IsControl(const char _main[]) 
{
	for (int i = 0; i < slen(_main); i++) { if (!iscntrl(_main[i])) { return false; } }
	return true;
}

#endif