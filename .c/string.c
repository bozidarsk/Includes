#include <stdlib.h>
#include <stdarg.h>
#include "string.h"

size_t Length(const char* str) 
{
	size_t length = 0;
	while (str[length] != 0x00) { length++; }
	return length;
}

bool IsNullOrEmpty(const char* str) { return str == NULL || str[0] == 0x00; }
bool Equal(const char* lStr, const char* rStr) 
{
	size_t lLStr = Length(lStr);
	size_t lRStr = Length(rStr);
	if (lLStr != lRStr) { return false; }
	for (size_t i = 0; i < lLStr; i++) { if (lStr[i] != rStr[i]) { return false; } }
	return true;
}

char* FillString(const char* str, const char target) { return FillString(str, target, 0, Length(str) + 1); }
char* FillString(const char* str, const char target, const size_t start, const size_t count) 
{
	size_t length = Length(str);
	char* output = NewString(length);
	for (size_t i = start; i < length; i++) 
	{
		if (i >= start && i < start + count) { output[i] = target; }
		else { output[i] = str[i]; }
	}

	return output;
}

char* NewString(const size_t length) 
{
	char* output = (char*)malloc(length + 1);
	output[length] = 0x00;
	return output;	
}

char* NewString(const size_t length, const char target) 
{
	char* output = (char*)malloc(length + 1);
	for (size_t i = 0; i < length; i++) { output[i] = target; }
	output[length] = 0x00;
	return output;	
}

char* EmptyString() 
{
	char* output = (char*)malloc(1);
	output[0] = 0x00;
	return output;
}

char* Copy(const char* str) 
{
	size_t length = Length(str);
	char* output = NewString(length);
	for (size_t i = 0; i < length; i++) { output[i] = str[i]; }
	output[length] = 0x00;
	return output;
}

char* Join(const char* str0, const char* str1, const char* str2) { return Join(str0, Join(str1, str2)); }
char* Join(const char* str0, const char* str1) 
{
	size_t index = 0;
	size_t length0 = Length(str0);
	size_t length1 = Length(str1);
	char* output = NewString(length0 + length1);

	for (int i = 0; i < length0; i++) { output[index++] = str0[i]; }
	for (int i = 0; i < length1; i++) { output[index++] = str1[i]; }

	output[length0 + length1] = 0x00;
	return output;
}

size_t LastIndexOf(const char* str, const char* target) { return LastIndexOf(str, target, Length(str) - 1); }
size_t LastIndexOf(const char* str, const char* target, const size_t start) 
{
	size_t lStr = Length(str);
	if (IsNullOrEmpty(str) || IsNullOrEmpty(target) || start < 0 || start >= lStr) { return -1; }
	size_t lTarget = Length(target);

	for (size_t i = start; i >= 0; i--) 
	{
		if (str[i] != target[lTarget - 1]) { continue; }

		size_t ii = i;
		size_t t = lTarget - 1;
		for (; t >= 0; t--) 
		{
			if (str[ii--] != target[t]) { break; }
			if (t == 0) { return i - lTarget + 1; }
		}

		if (i == 0) { break; }
	}

	return -1;
}

size_t IndexOf(const char* str, const char* target) { return IndexOf(str, target, 0); }
size_t IndexOf(const char* str, const char* target, const size_t start) 
{
	size_t lStr = Length(str);
	if (IsNullOrEmpty(str) || IsNullOrEmpty(target) || start < 0 || start >= lStr) { return -1; }
	size_t lTarget = Length(target);

	for (size_t i = start; i < lStr; i++) 
	{
		if (str[i] != target[0]) { continue; }

		size_t t = 0;
		for (; t < lTarget; t++) { if (str[i + t] != target[t]) { break; } }
		if (t >= lTarget) { return i; }
	}

	return -1;
}

char* Reverse(const char* str) 
{
	if (IsNullOrEmpty(str)) { return Copy(str); }

	size_t length = Length(str);
	char* output = Copy(str);
	for (size_t i = 0; i < length / 2; i++) 
	{
		char tmp = output[length - 1 - i];
		output[length - 1 - i] = output[i];
		output[i] = tmp;
	}

	output[length] = 0x00;
	return output;
}

char* Remove(const char* str, const size_t start, const size_t count) 
{
	size_t length = Length(str);
	if (IsNullOrEmpty(str) || start < 0 || start >= length || count <= 0 || start + count > length) { return Copy(str); }

	size_t index = 0;
	char* output = NewString(length - count);

	for (size_t i = 0; i < length; i++) 
	{
		if (i >= start && i < start + count) { continue; }
		output[index++] = str[i];
	}

	output[length - count] = 0x00;
	return output;
}

char* Insert(const char* str, const char* target, const size_t index) 
{
	size_t lStr = Length(str);
	if (IsNullOrEmpty(str) || IsNullOrEmpty(target) || index < 0 || index >= lStr) { return Copy(str); }

	size_t lTarget = Length(target);
	char* output = NewString(lStr + lTarget);

	size_t t = 0;
	size_t s = 0;
	for (size_t i = 0; i < lStr + lTarget; i++) 
	{
		if (i >= index && i < index + lTarget) { output[i] = target[t++]; continue; }
		output[i] = str[s++];
	}

	output[lStr + lTarget] = 0x00;
	return output;
}

char* Replace(const char* str, const char* from, const char* to) { return Replace(str, from, to, Length(str) + 1); }
char* Replace(const char* str, const char* from, const char* to, const size_t count) 
{
	if (IsNullOrEmpty(str) || IsNullOrEmpty(from) || count <= 0) { return Copy(str); }

	size_t index = IndexOf(str, from, 0);
	if (index < 0) { return Copy(str); }

	char* output = Remove(str, index, Length(from));
	output = Insert(output, to, index);

	return (count == 1) ? output : Replace(output, from, to, count - 1);
}

char* TrimStart(const char* str, const size_t count) 
{
	size_t length = Length(str);
	if (IsNullOrEmpty(str) || count <= 0 || count > length) { return Copy(str); }
	return Remove(str, 0, count);
}

char* TrimStart(const char* str, const char target) 
{
	size_t length = Length(str);
	if (IsNullOrEmpty(str) || target == 0x00) { return Copy(str); }

	size_t i = 0;
	for (; i < length; i++) { if (str[i] != target) { break; } }
	return Remove(str, 0, i);
}

char* TrimEnd(const char* str, const size_t count) 
{
	size_t length = Length(str);
	if (IsNullOrEmpty(str) || count <= 0 || count > length) { return Copy(str); }
	return Remove(str, length - count, count);
}

char* TrimEnd(const char* str, const char target) 
{
	size_t length = Length(str);
	if (IsNullOrEmpty(str) || target == 0x00) { return Copy(str); }

	size_t i = length - 1;
	for (; i >= 0; i--) { if (str[i] != target || i == 0) { break; } }
	return Remove(str, i + 1, length - i + 1);
}

bool Contains(const char* str, const char* target) { return IndexOf(str, target, 0) >= 0; }
bool StartsWith(const char* str, const char* target) { return IndexOf(str, target) == 0; }
bool EndsWith(const char* str, const char* target) { return LastIndexOf(str, target) == Length(str) - Length(target); }

char* GetStringAt(const char* str, const size_t start, const size_t end) { return Substring(str, start, (end - start) + 1); }
char* Substring(const char* str, const size_t start, const size_t count) 
{
	size_t length = Length(str);
	if (IsNullOrEmpty(str) || start < 0 || start >= length || count <= 0 || start + count > length) { return Copy(str); }

	size_t index = 0;
	char* output = NewString(count);

	for (size_t i = 0; i < length; i++) 
	{
		if (!(i >= start && i < start + count)) { continue; }
		output[index++] = str[i];
	}

	output[count] = 0x00;
	return output;
}

char* ToString(const char x) { return NewString(1, x); }
char* ToString(const int x) 
{
	int num = x;
	if (num == 0) { return ToString('0'); }

	int _num = num;
	size_t length = 0;
	while (_num != 0) { _num /= 10; length++; }

	bool isNegative = false;
	if (num < 0) { isNegative = true; num *= -1; }

	char* output = NewString(length + isNegative);
	for (size_t i = length - 1; i >= 0 + isNegative; i--) 
	{
		output[i] = (num % 10) + 0x30;
		num /= 10;
		if (i == 0 + isNegative) { break; }
	}

	if (isNegative) { output[0] = '-'; }
	output[length] = 0x00;
	return output;
}

char ToLower(const char x) { return (x >= 0x41 && x <= 0x5a) ? x + 0x20 : x; }
char ToUpper(const char x) { return (x >= 0x61 && x <= 0x7a) ? x - 0x20 : x; }
bool IsAlphanumeric(const char x) { return (x >= 0x30 && x <= 0x39) || (x >= 0x41 && x <= 0x5a) || (x >= 0x61 && x <= 0x7a); }
bool IsAlphabet(const char x) { return (x >= 0x41 && x <= 0x5a) || (x >= 0x61 && x <= 0x7a); }
bool IsNumber(const char x) { return x >= 0x30 && x <= 0x39; }
bool IsHex(const char x) { return (x >= 0x30 && x <= 0x39) || (x >= 0x41 && x <= 0x46) || (x >= 0x61 && x <= 0x66); }
bool IsControl(const char x) { return x >= 0x00 && x <= 0x1f; }
bool IsUpper(const char x) { return x == ToUpper(x); }
bool IsLower(const char x) { return x == ToLower(x); }

char* ToLower(const char* str) 
{
	char* output = Copy(str);
	size_t length = Length(str);
	for (size_t i = 0; i < length; i++) { output[i] = ToLower(output[i]); }
	return output;
}

char* ToUpper(const char* str) 
{
	char* output = Copy(str);
	size_t length = Length(str);
	for (size_t i = 0; i < length; i++) { output[i] = ToUpper(output[i]); }
	return output;
}

bool IsNumber(const char* str) 
{
	bool dot = false;
	size_t length = Length(str);
	for (size_t i = 0; i < length; i++) 
	{
		if (i == 0 && str[i] == '-') { continue; }
		if (str[i] == '.' && dot) { return false; }
		if (str[i] == '.') { dot = true; continue; }
		if (!IsNumber(str[i])) { return false; }
	}

	return true;
}

bool IsAlphanumeric(const char* str) 
{
	size_t length = Length(str);
	for (size_t i = 0; i < length; i++) { if (!IsAlphanumeric(str[i])) { return false; } }
	return true;
}

bool IsAlphabet(const char* str) 
{
	size_t length = Length(str);
	for (size_t i = 0; i < length; i++) { if (!IsAlphabet(str[i])) { return false; } }
	return true;
}

bool IsHex(const char* str) 
{
	size_t length = Length(str);
	for (size_t i = 0; i < length; i++) { if (!IsHex(str[i])) { return false; } }
	return true;
}

bool IsControl(const char* str) 
{
	size_t length = Length(str);
	for (size_t i = 0; i < length; i++) { if (!IsControl(str[i])) { return false; } }
	return true;
}

bool IsUpper(const char* str) 
{
	size_t length = Length(str);
	for (size_t i = 0; i < length; i++) { if (!IsUpper(str[i])) { return false; } }
	return true;
}

bool IsLower(const char* str) 
{
	size_t length = Length(str);
	for (size_t i = 0; i < length; i++) { if (!IsLower(str[i])) { return false; } }
	return true;
}