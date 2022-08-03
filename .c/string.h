#ifndef STRING_H

#define STRING_H
#define EMPTYSTR ""
#define EMPTYCHAR '\0'
#pragma GCC diagnostic ignored "-Wformat="
#pragma GCC diagnostic ignored "-Wformat-security"
char _EMPTYCHAR = '\x0';

#ifndef _CTYPE_H
#if __has_include("ctype.h")
#include <ctype.h>
#endif
#endif

int slen(const char _main[]) 
{
	int i = 0;
	while (_main[i] != '\0') { i++; }
	return i;
}

typedef struct string
{
	char* array;
	int Length = 0;

	operator char*() const { return array; }
	char& operator [] (const int index) { if (index < 0 || index >= Length) { return _EMPTYCHAR; } return array[index]; }

	void operator = (const char* str) { Length = slen(str); array = (char*)malloc(Length); for (int i = 0; i < Length; i++) { array[i] = str[i]; }  }
	void operator = (string str) { Length = str.Length; array = (char*)malloc(Length); for (int i = 0; i < Length; i++) { array[i] = str[i]; }  }

	bool operator == (const char* str) { if (slen(str) != Length) { return false; } for (int i = 0; i < Length; i++) { if (array[i] != str[i]) { return false; } } return true; }
	bool operator == (string str) { if (str.Length != Length) { return false; } for (int i = 0; i < Length; i++) { if (array[i] != str[i]) { return false; } } return true; }

	bool operator != (const char* str) { if (slen(str) != Length) { return true; } for (int i = 0; i < Length; i++) { if (array[i] != str[i]) { return true; } } return false; }
	bool operator != (string str) { if (str.Length != Length) { return true; } for (int i = 0; i < Length; i++) { if (array[i] != str[i]) { return true; } } return false; }

	void operator += (const char chr) 
	{
		if (chr == EMPTYCHAR) { return; }
		char* newArray = (char*)malloc(Length + 1);
		for (int i = 0; i < Length; i++) { newArray[i] = array[i]; }
		newArray[Length] = chr;
		array = newArray;
		Length++;
	}

	void operator += (const	char* str) 
	{
		int Length2 = slen(str);
		if (Length2 <= 0) { return; }
		char* newArray = (char*)malloc(Length + Length2);
		for (int i = 0; i < Length + Length2; i++) 
		{
			if (i < Length) { newArray[i] = array[i]; }
			else { newArray[i] = str[i - Length]; }
		}

		Length += Length2;	
		array = newArray;
	}

	void operator += (string str) 
	{
		if (str.Length <= 0) { return; }
		char* newArray = (char*)malloc(Length + str.Length);
		for (int i = 0; i < Length + str.Length; i++) 
		{
			if (i < Length) { newArray[i] = array[i]; }
			else { newArray[i] = str[i - Length]; }
		}
	
		Length += str.Length;
		array = newArray;
	}

	string operator + (const char chr) 
	{
		if (chr == EMPTYCHAR) { string s; s = EMPTYSTR; return s; }
		char* newArray = (char*)malloc(Length + 1);
		for (int i = 0; i < Length; i++) { newArray[i] = array[i]; }
		newArray[Length] = chr;

		string output;
		output = newArray;
		return output;
	}

	string operator + (const char* str) 
	{
		int Length2 = slen(str);
		if (Length2 <= 0) { string s; s = EMPTYSTR; return s; }
		char* newArray = (char*)malloc(Length + Length2);
		for (int i = 0; i < Length + Length2; i++) 
		{
			if (i < Length) { newArray[i] = array[i]; }
			else { newArray[i] = str[i - Length]; }
		}

		string output;
		output = newArray;
		return output;
	}

	string operator + (string str) 
	{
		if (str.Length <= 0) { string s; s = EMPTYSTR; return s; }
		char* newArray = (char*)malloc(Length + str.Length);
		for (int i = 0; i < Length + str.Length; i++) 
		{
			if (i < Length) { newArray[i] = array[i]; }
			else { newArray[i] = str[i - Length]; }
		}

		string output;
		output = newArray;
		return output;
	}
}string;

string s(const char str[]) { string output; output = str; return output; }
bool IsNullOrEmpty(string str) { return (str.Length <= 0 || str[0] == EMPTYCHAR) ? true : false; }
string EmptyString() { string output; output = EMPTYSTR; return output; }
string ToString(const char _main);

string Remove(string _main, const int startIndex) 
{
	if (IsNullOrEmpty(_main) || startIndex < 0 || startIndex >= _main.Length) { return _main; }

	int i = 0;
	string output;
	while (i < startIndex) { output += _main[i]; i++; }
	return output;
}

string Remove(string _main, const int startIndex, const int count) 
{
	if (IsNullOrEmpty(_main) || startIndex + count > _main.Length || count <= 0 || count > _main.Length || startIndex < 0 || startIndex >= _main.Length) { return _main; }

	int i = 0;
	string output;
	while (i < _main.Length) 
	{
		if (i == startIndex) { i += count; }
		output += _main[i];
		i++;
	}

	return output;
}

string Reverse(string _main) 
{
    if (IsNullOrEmpty(_main)) { return _main; }
    string main;
    main = _main;

    int x = 0;
    int y = main.Length - 1;
    char rightChar = '0';
    while (x < main.Length / 2) 
    {
    	rightChar = main[y];
    	main[y] = main[x];
    	main[x] = rightChar;
    	x++;
    	y--;
    }

    return main;
}

string GetStringAt(string _main, const int startPos, const int endPos) 
{
    if (IsNullOrEmpty(_main) || startPos < 0 || endPos <= 0 || endPos < startPos || endPos >= _main.Length) { return EmptyString(); }

    int i = startPos;
    string output;
    while (i <= endPos) { output += _main[i]; i++; }
    return output;
}

bool StartsWith(string _main, string _target) 
{
	if (IsNullOrEmpty(_main) || IsNullOrEmpty(_target) || _target.Length > _main.Length) { return false; }

    int i = 0;
    bool pass = true;

    if (_main.Length < _target.Length) { pass = false; }
    while (i < _target.Length && pass) 
    {
        if (_main[i] != _target[i]) { pass = false; }
        i++;
    }

    return pass;
}

bool EndsWith(string _main, string _target) 
{
	if (IsNullOrEmpty(_main) || IsNullOrEmpty(_target) || _target.Length > _main.Length) { return false; }

    int i = _main.Length - 1;
    int t = _target.Length - 1;
    bool pass = true;

    if (_main.Length < _target.Length) { pass = false; }
    while (i >= _main.Length - _target.Length && pass) 
    {
        if (_main[i] != _target[t]) { pass = false; }
        i--;
        t--;
    }

    return pass;
}

int IndexOf(string _main, string _target) 
{
	if (IsNullOrEmpty(_main) || IsNullOrEmpty(_target) || _target.Length > _main.Length) { return -1; }

	int i = 0;
	int t = 0;
	while (i < _main.Length) 
	{
		t = 0;
		while (t < _target.Length) 
		{
			if (_main[i + t] != _target[t]) { break; }
			t++;
		}

		if (t >= _target.Length) { return i; }
		i++;
	}

	return -1;
}

int IndexOf(string _main, string _target, const int startIndex) 
{
	if (IsNullOrEmpty(_main) || IsNullOrEmpty(_target) || _target.Length > _main.Length || startIndex < 0 || startIndex >= _main.Length) { return -1; }

	int i = startIndex;
	int t = 0;
	while (i < _main.Length) 
	{
		t = 0;
		while (t < _target.Length) 
		{
			if (_main[i + t] != _target[t]) { break; }
			t++;
		}

		if (t >= _target.Length) { return i; }
		if ((i + 1) + _target.Length >= _main.Length) { return -1; }
		i++;
	}

	return -1;
}

int LastIndexOf(string _main, string _target) 
{
	if (IsNullOrEmpty(_main) || IsNullOrEmpty(_target) || _target.Length > _main.Length) { return -1; }

	int i = _main.Length - _target.Length;
	int t = 0;
	while (i >= 0) 
	{
		t = 0;
		while (t < _target.Length) 
		{
			if (_main[i + t] != _target[t]) { break; }
			t++;
		}

		if (t >= _target.Length) { return i; }
		if (i - _target.Length < 0) { return -1; }
		i--;
	}

	return -1;
}

bool Contains(string _main, string _target) 
{
	if (IsNullOrEmpty(_main) || IsNullOrEmpty(_target) || _target.Length > _main.Length) { return false; }

	int i = 0;
	int t = 0;
	while (i < _main.Length) 
	{
		t = 0;
		while (t < _target.Length) 
		{
			if (_main[i + t] != _target[t]) { break; }
			t++;
		}

		if (t >= _target.Length) { return true; }
		if ((i + 1) + _target.Length >= _main.Length) { return false; }
		i++;
	}

	return false;
}

int Count(string _main, const char _target) 
{
	if (IsNullOrEmpty(_main) || _target == EMPTYCHAR) { return 0; }

	int count = 0;
	for (int i = 0; i < _main.Length; i++) { count += (_main[i] == _target) ? 1 : 0; }
	return count;
}

int Count(string _main, string _target) 
{
	if (IsNullOrEmpty(_main) || IsNullOrEmpty(_target) || _target.Length > _main.Length) { return 0; }

	int i = IndexOf(_main, _target);
	int count = 0;

	while (i >= 0) 
	{
		i = IndexOf(_main, _target, i + 1);
		count++;
	}

	return count;
}

string Insert(string _main, string _target, const int index) 
{
	if (IsNullOrEmpty(_main) || IsNullOrEmpty(_target) || index < 0 || index >= _main.Length) { return _main; }

	int i = 0;
	string output;
	while (i < _main.Length) 
	{
		if (i == index) { output += _target; }
		output += _main[i];
		i++;
	}

	return output;
}

string Replace(string _main, string _from, string _to) 
{
	if (IsNullOrEmpty(_main) || IsNullOrEmpty(_from) || IsNullOrEmpty(_to) || _from.Length > _main.Length) { return _main; }

	int i = 0;
	string output;
	while (i < _main.Length) 
	{
		if (i == IndexOf(_main, _from, i)) { output += _to; i += _from.Length; }
		output += _main[i];
		i++;
	}

	return output;
}

string Replace(string _main, string _from, string _to, const int count) 
{
	if (count <= 0 || IsNullOrEmpty(_main) || IsNullOrEmpty(_from) || IsNullOrEmpty(_to) || _from.Length > _main.Length) { return _main; }

	int i = 0;
	int c = count;
	string output;
	while (i < _main.Length) 
	{
		if (i == IndexOf(_main, _from, i) && c > 0) { output += _to; i += _from.Length; c--; }
		output += _main[i];
		i++;
	}

	return output;
}

string FillString(string _main, const char _target) 
{
	if (IsNullOrEmpty(_main) || _target == EMPTYCHAR) { return _main; }

	string output;
	output = _main;
	for (int i = 0; i < output.Length; i++) { output[i] = _target; }
	return output;
}

string FillString(const char _target, const int count) 
{
	if (_target == EMPTYCHAR || count <= 0) { return EmptyString(); }

	string output;
	for (int i = 0; i < count; i++) { output += _target; }
	return output;
}

string TrimStart(string _main, const int count) 
{
	if (IsNullOrEmpty(_main) || count <= 0 || count > _main.Length) { return _main; }

	string output;
	for (int i = 0; i < _main.Length - count; i++) { output += _main[i + count]; }
	return output;
}

string TrimStart(string _main, const char _target) 
{
	if (IsNullOrEmpty(_main) || _target == EMPTYCHAR || !StartsWith(_main, ToString(_target))) { return _main; }

	int i = 0;
	while (i < _main.Length) { if (_main[i] != _target) { break; } i++; }
	return TrimStart(_main, i);
}

string TrimEnd(string _main, const int count) 
{
	if (IsNullOrEmpty(_main) || count <= 0 || count > _main.Length) { return _main; }

	string output;
	for (int i = 0; i < _main.Length - count; i++) { output += _main[i]; }
	return output;
}

string TrimEnd(string _main, const char _target) 
{
	if (IsNullOrEmpty(_main) || _target == EMPTYCHAR || !EndsWith(_main, ToString(_target))) { return _main; }

	int i = _main.Length - 1;
	while (i >= 0) { if (_main[i] != _target) { break; } i--; }
	return TrimEnd(_main, _main.Length - i - 1);
}

string ToString(const char _main) 
{
	if (_main == EMPTYCHAR) { return EmptyString(); }
	string output;
	output += _main;
	return output;
}

string ToString(const float _main) 
{
	int digits = 0;
	float result = _main;
	while (result / 10 > 0) { result /= 10; digits++; }

	string output;
	char* formated = (char*)malloc(digits);
	sprintf(formated, "%f", _main);
	output = formated;
	return TrimEnd(output, '0');
}

string ToString(const double _main) 
{
	int digits = 0;
	double result = _main;
	while (result / 10 > 0) { result /= 10; digits++; }

	string output;
	char* formated = (char*)malloc(digits);
	sprintf(formated, "%f", _main);
	output = formated;
	return TrimEnd(output, '0');
}

string ToString(const int _main) 
{
	int digits = 0;
	int result = _main;
	while (result % 10 != 0) { result /= 10; digits++; }

	string output;
	char* formated = (char*)malloc(digits);
	sprintf(formated, "%i", _main);
	output = formated;
	return output;
}

char ToUpper(const char _main) { return (char)toupper(_main); }
string ToUpper(string _main) 
{
	if (IsNullOrEmpty(_main)) { return _main; }

	string output;
	for (int i = 0; i < _main.Length; i++) { output += (char)toupper(_main[i]); }
	return output;
}

char ToLower(const char _main) { return (char)tolower(_main); }
string ToLower(string _main) 
{
	if (IsNullOrEmpty(_main)) { return _main; }
	
	string output;
	for (int i = 0; i < _main.Length; i++) { output += (char)tolower(_main[i]); }
	return output;
}

bool IsUpper(const char _main) { return isupper(_main); }
bool IsUpper(string _main) 
{
	for (int i = 0; i < _main.Length; i++) { if (!isupper(_main[i])) { return false; } }
	return true;
}

bool IsLower(const char _main) { return islower(_main); }
bool IsLower(string _main) 
{
	for (int i = 0; i < _main.Length; i++) { if (!islower(_main[i])) { return false; } }
	return true;
}

bool IsDigit(const char _main) { return isdigit(_main); }
bool IsDigit(string _main) 
{
	for (int i = 0; i < _main.Length; i++) { if (!isdigit(_main[i])) { return false; } }
	return true;
}

bool IsAlphabetic(const char _main) { return isalpha(_main); }
bool IsAlphabetic(string _main) 
{
	for (int i = 0; i < _main.Length; i++) { if (!isalpha(_main[i])) { return false; } }
	return true;
}

bool IsPrintable(const char _main) { return isprint(_main); }
bool IsPrintable(string _main) 
{
	for (int i = 0; i < _main.Length; i++) { if (!isprint(_main[i])) { return false; } }
	return true;
}

bool IsControl(const char _main) { return iscntrl(_main); }
bool IsControl(string _main) 
{
	for (int i = 0; i < _main.Length; i++) { if (!iscntrl(_main[i])) { return false; } }
	return true;
}

#if __has_include("charstring.h")
#include "charstring.h"
#endif

#endif