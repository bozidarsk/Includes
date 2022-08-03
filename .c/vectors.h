#ifndef VECTORS_H

#define VECTORS_H

typedef struct float4
{
	float x, y, z, w;

	bool operator == (const float4 v) { return (v.x == x && v.y == y && v.z == z && v.w == w); }
	bool operator != (const float4 v) { return !(v.x == x && v.y == y && v.z == z && v.w == w); }
	void operator *= (const float n) { x *= n; y *= n; z *= n; w *= n; }
	void operator /= (const float n) { x /= n; y /= n; z /= n; w /= n; }
	float4 operator * (const float n) { float4 a = { x * n, y * n, z * n, w * n }; return a; }
	float4 operator / (const float n) { float4 a = { x / n, y / n, z / n, w / n }; return a; }
	void operator += (const float4 v) { x += v.x; y += v.y; z += v.z; w += v.w; };
	void operator -= (const float4 v) { x -= v.x; y -= v.y; z -= v.z; w -= v.w; };
	float4 operator + (const float4 v) { float4 a = { x + v.x, y + v.y, z + v.z, w + v.w }; return a; };
	float4 operator - (const float4 v) { float4 a = { x - v.x, y - v.y, z - v.z, w - v.w }; return a; };
} float4;

typedef struct float3
{
	float x, y, z;

	bool operator == (const float3 v) { return (v.x == x && v.y == y && v.z == z); }
	bool operator != (const float3 v) { return !(v.x == x && v.y == y && v.z == z); }
	void operator *= (const float n) { x *= n; y *= n; z *= n; }
	void operator /= (const float n) { x /= n; y /= n; z /= n; }
	float3 operator * (const float n) { float3 a = { x * n, y * n, z * n }; return a; }
	float3 operator / (const float n) { float3 a = { x / n, y / n, z / n }; return a; }
	void operator += (const float3 v) { x += v.x; y += v.y; z += v.z; };
	void operator -= (const float3 v) { x -= v.x; y -= v.y; z -= v.z; };
	float3 operator + (const float3 v) { float3 a = { x + v.x, y + v.y, z + v.z }; return a; };
	float3 operator - (const float3 v) { float3 a = { x - v.x, y - v.y, z - v.z }; return a; };
} float3;

typedef struct float2
{
	float x, y;

	bool operator == (const float2 v) { return (v.x == x && v.y == y); }
	bool operator != (const float2 v) { return !(v.x == x && v.y == y); }
	void operator *= (const float n) { x *= n; y *= n; }
	void operator /= (const float n) { x /= n; y /= n; }
	float2 operator * (const float n) { float2 a = { x * n, y * n }; return a; }
	float2 operator / (const float n) { float2 a = { x / n, y / n }; return a; }
	void operator += (const float2 v) { x += v.x; y += v.y; };
	void operator -= (const float2 v) { x -= v.x; y -= v.y; };
	float2 operator + (const float2 v) { float2 a = { x + v.x, y + v.y }; return a; };
	float2 operator - (const float2 v) { float2 a = { x - v.x, y - v.y }; return a; };
} float2;

typedef struct int4
{
	int32 x, y, z, w;

	bool operator == (const int4 v) { return (v.x == x && v.y == y && v.z == z && v.w == w); }
	bool operator != (const int4 v) { return !(v.x == x && v.y == y && v.z == z && v.w == w); }
	void operator *= (const int32 n) { x *= n; y *= n; z *= n; w *= n; }
	void operator /= (const int32 n) { x /= n; y /= n; z /= n; w /= n; }
	int4 operator * (const int32 n) { int4 a = { x * n, y * n, z * n, w * n }; return a; }
	int4 operator / (const int32 n) { int4 a = { x / n, y / n, z / n, w / n }; return a; }
	void operator += (const int4 v) { x += v.x; y += v.y; z += v.z; w += v.w; };
	void operator -= (const int4 v) { x -= v.x; y -= v.y; z -= v.z; w -= v.w; };
	int4 operator + (const int4 v) { int4 a = { x + v.x, y + v.y, z + v.z, w + v.w }; return a; };
	int4 operator - (const int4 v) { int4 a = { x - v.x, y - v.y, z - v.z, w - v.w }; return a; };
} int4;

typedef struct int3
{
	int32 x, y, z;

	bool operator == (const int3 v) { return (v.x == x && v.y == y && v.z == z); }
	bool operator != (const int3 v) { return !(v.x == x && v.y == y && v.z == z); }
	void operator *= (const int32 n) { x *= n; y *= n; z *= n; }
	void operator /= (const int32 n) { x /= n; y /= n; z /= n; }
	int3 operator * (const int32 n) { int3 a = { x * n, y * n, z * n }; return a; }
	int3 operator / (const int32 n) { int3 a = { x / n, y / n, z / n }; return a; }
	void operator += (const int3 v) { x += v.x; y += v.y; z += v.z; };
	void operator -= (const int3 v) { x -= v.x; y -= v.y; z -= v.z; };
	int3 operator + (const int3 v) { int3 a = { x + v.x, y + v.y, z + v.z }; return a; };
	int3 operator - (const int3 v) { int3 a = { x - v.x, y - v.y, z - v.z }; return a; };
} int3;

typedef struct int2
{
	int32 x, y;

	bool operator == (const int2 v) { return (v.x == x && v.y == y); }
	bool operator != (const int2 v) { return !(v.x == x && v.y == y); }
	void operator *= (const int32 n) { x *= n; y *= n; }
	void operator /= (const int32 n) { x /= n; y /= n; }
	int2 operator * (const int32 n) { int2 a = { x * n, y * n }; return a; }
	int2 operator / (const int32 n) { int2 a = { x / n, y / n }; return a; }
	void operator += (const int2 v) { x += v.x; y += v.y; };
	void operator -= (const int2 v) { x -= v.x; y -= v.y; };
	int2 operator + (const int2 v) { int2 a = { x + v.x, y + v.y }; return a; };
	int2 operator - (const int2 v) { int2 a = { x - v.x, y - v.y }; return a; };
} int2;

float4 f4(const float x, const float y, const float z, const float w) { float4 v = { x, y, z, w }; return v; }
float3 f3(const float x, const float y, const float z) { float3 v = { x, y, z }; return v; }
float2 f2(const float x, const float y) { float2 v = { x, y }; return v; }

int4 i4(const int x, const int y, const int z, const int w) { int4 v = { x, y, z, w }; return v; }
int3 i3(const int x, const int y, const int z) { int3 v = { x, y, z }; return v; }
int2 i2(const int x, const int y) { int2 v = { x, y }; return v; }

#endif