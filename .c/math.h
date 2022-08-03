#ifndef MATH_H

#define MATH_H
#include "vectors.h"
#define TAU 6.283185307179586476925286766559
#define PI 3.1415926535897932384626433832795
#define E 2.7182818284590452353602874713527
#define goldenRatio 1.6180339887498948482045868343657
#define DEG2RAD 0.01745329251994329576923690768489
#define RAD2DEG 57.295779513082320876798154814105

float sqrt(const float x) 
{
    if (x < 0) { return 0; }
    union 
    {
        int i;
        float x;
    } u;

    u.x = x;
    u.i = (1<<29) + (u.i >> 1) - (1<<22); 

    u.x =       u.x + x/u.x;
    u.x = 0.25f*u.x + x/u.x;
    return u.x;
}

float sin(const float x) { return x - (x*x*x / 6) + (x*x*x*x*x / 120) - (x*x*x*x*x*x*x / 5040) + (x*x*x*x*x*x*x*x*x / 362880) - (x*x*x*x*x*x*x*x*x*x*x / 39916800) + (x*x*x*x*x*x*x*x*x*x*x*x*x / 6227020800) - (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 1307674368000) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 355687428096000) - (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 121645100408832000); }
float cos(const float x) { return 1 - (x*x / 2) + (x*x*x*x / 24) - (x*x*x*x*x*x / 720) + (x*x*x*x*x*x*x*x / 40320) - (x*x*x*x*x*x*x*x*x*x / 3628800) + (x*x*x*x*x*x*x*x*x*x*x*x / 479001600) - (x*x*x*x*x*x*x*x*x*x*x*x*x*x / 87178291200) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 20922789888000) - (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 6402373705728000) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 2432902008176640000); }
float tan(const float x) { return sin(x) / cos(x); }

float asin(const float x) { return 1 / sin(x); }
float acos(const float x) { return 1 / cos(x); }
float atan(const float x) { return 1 / tan(x); }

// float asin(const float x) { return x + (x*x*x* 0.16666666666666666) + (x*x*x*x*x* 0.075) + (x*x*x*x*x*x*x* 0.044642857142857144) + (x*x*x*x*x*x*x*x*x* 0.030381944444444444) + (x*x*x*x*x*x*x*x*x*x*x* 0.022372159090909092) + (x*x*x*x*x*x*x*x*x*x*x*x*x* 0.017352764423076924) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.01396484375) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.011551800896139705) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.009761609529194078) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.008390335809616815) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.0073125258735988454) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.006447210311889649) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.005740037670841924) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.005153309682319905) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.004660143486915096) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.004240907093679363) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.003880964558837669) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.0035692053938259347) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.003297059503473485) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.0030578216492580306) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.002846178401108942) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.00265787063820729) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.0024894486782468836) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.002338091892111975) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.0022014739737101384) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.0020776610325181676) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.0019650336162772837) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.0018622264064031275) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.0017680811205154183) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.0016816093935831068) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.001601963275351444) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.0015284115961225677) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.0014603208940791154) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.0013971399176302534) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.0013383869512751784) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.0012836393876290285) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.001232525098500017) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.0011847152561624392) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.0011399183307022236) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.0010978750465914472) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.0010583541258722428) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.0010211486797106276) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.0009860731369833312) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.0009529606197429564) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.0009216606921836334) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.0008920374230917097) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.0008639677124658675) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.0008373398416027121) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.0008120522129086701) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x* 0.0007880122513582056); }

// float acos(const float x) 
// {
//     float a = 1.43 + 0.59 * x;
//     float b = 1.65 - 1.41 * x;
//     float c = 0.88 - 0.77 * x;
//     a = (a + (2 + 2 * x) / a) / 2;
//     b = (b + (2 - 2 * x) / b) / 2;
//     c = (c + (2 - a ) / c) / 2 ;
//     return (8*(c+(2-a)/c)-(b+(2-2*x)/b))/6;
// }

// float atan(const float x) { return x - (x*x*x / 3) + (x*x*x*x*x / 5) - (x*x*x*x*x*x*x / 7) + (x*x*x*x*x*x*x*x*x / 9) - (x*x*x*x*x*x*x*x*x*x*x / 11) + (x*x*x*x*x*x*x*x*x*x*x*x*x / 13) - (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 15) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 17) - (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 19) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 21) - (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 23) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 25) - (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 27) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 29) - (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 31) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 33) - (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 35) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 37) - (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 39) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 41) - (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 43) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 45) - (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 47) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 49) - (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 51) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 53) - (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 55) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 57) - (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 59) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 61) - (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 63) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 65) - (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 67) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 69) - (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 71) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 73) - (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 75) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 77) - (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 79) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 81) - (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 83) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 85) - (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 87) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 89) - (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 91) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 93) - (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 95) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 97) - (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 99) + (x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x*x / 101); }
float atan2(const float y, const float x) { return 2 * atan(y / (sqrt((x*x) + (y*y)) + x)); }

int fact(const int x) 
{
    int n = x;
    int result = 1;
    while (n > 0) { result *= n; n--; }
    return result;
}

float pow10(const int x) 
{
    float n = abs(x);
    float result = 1;
    while (n > 0) { result *= 10; n--; }
    return (x < 0) ? 1 / result : result;
}

float exp(const float x) { return pow(E, x); }
float copysign(const float a, const float b) { if (a == 0 || b == 0) { return a; } if ((a < 0 && b < 0) || (a > 0 && b > 0)) { return a; } else { return a * -1; } }
float lerp(const float a, const float b, const float x) { return a + (x * (b - a)); }
float2 lerp(const float2 a, const float2 b, const float x) { return f2(lerp(a.x, b.x, x), lerp(a.y, b.y, x)); }
float3 lerp(const float3 a, const float3 b, const float x) { return f3(lerp(a.x, b.x, x), lerp(a.y, b.y, x), lerp(a.z, b.z, x)); }
float inverseLerp(const float a, const float b, const float x) { return (x - a) / (b - a); }
float max(const float a, const float b) { return (a > b) ? a : b; }
float min(const float a, const float b) { return (a < b) ? a : b; }
float clamp(const float a, const float b, const float x) { return max(a, min(x, b)); }
float abs(const float x) { return (x < 0) ? x * -1 : x; }
float smoothMin(const float a, const float b, const float x) { float t = clamp(0, 1, (b - a + x) / (2 * x)); return a * t + b * (1 - t) - x * t * (1 - t); }
float smoothMax(const float a, const float b, const float x) { return smoothMin(a, b, x * -1); }
float smoothstep(const float a, const float b, const float x) { float t = clamp(0, 1, (x - a) / (b - a)); return t * t * (3 - (2 * t)); }
float ceiling(const float x) { return (int)x + 1; }
float ceil(const float x) { return ceil(x); }
float floor(const float x) { return (int)x; }

float distance(const float3 a, const float3 b) { return sqrt(((b.x - a.x) * (b.x - a.x)) + ((b.y - a.y) * (b.y - a.y)) + ((b.z - a.z) * (b.z - a.z))); }
float distance(const float2 a, const float2 b) { return sqrt(((b.x - a.x) * (b.x - a.x)) + ((b.y - a.y) * (b.y - a.y))); }
float length(const float3 a) { return sqrt((a.x * a.x) + (a.y * a.y) + (a.z * a.z)); }
float length(const float2 a) { return sqrt((a.x * a.x) + (a.y * a.y)); }
float3 normalize(const float3 a) { float3 v = a; return v / length(v); }
float2 normalize(const float2 a) { float2 v = a; return v / length(v); }
float dot(const float3 a, const float3 b) { return (a.x * b.x) + (a.y * b.y) + (a.z * b.z); }
float dot(const float2 a, const float2 b) { return (a.x * b.x) + (a.y * b.y); }
float3 cross(const float3 a, const float3 b) { return f3((a.y * b.z) - (a.z * b.y), (a.z * b.x) - (a.x * b.z), (a.x * b.y) - (a.y * b.x)); }

float2 RaySphere(const float3 center, const float radius, const float3 rayOrigin, const float3 rayDir) // returns f2(distance to sphere, distance inside sphere)
{
    float a = 1;
    float3 offset = f3(rayOrigin.x - center.x, rayOrigin.y - center.y, rayOrigin.z - center.z);
    float b = 2 * dot(offset, rayDir);
    float c = dot(offset, offset) - radius * radius;

    float disciminant = b * b - 4 * a * c;

    if (disciminant > 0) 
    {
        float s = sqrt(disciminant);
        float dstToSphereNear = max(0, (-b - s) / (2 * a));
        float dstToShpereFar = (-b + s) / (2 * a);

        if (dstToShpereFar >= 0) 
        {
            return f2(dstToSphereNear, dstToShpereFar - dstToSphereNear);
        }
    }

    return f2(-1, 0);
}

float2 Rotate(const float2 origin, const float2 point, const float angle) 
{
    float x = origin.x + ((point.x - origin.x) * cos(angle)) + ((point.y - origin.y) * sin(angle));
    float y = origin.y + ((point.x - origin.x) * sin(angle)) + ((point.y - origin.y) * cos(angle));

    return f2(x, y);
}

float3 MidPoint(const float3 a, const float3 b) { return f3((a.x + b.x) / 2, (a.y + b.y) / 2, (a.z + b.z) / 2); }
float2 MidPoint(const float2 a, const float2 b) { return f2((a.x + b.x) / 2, (a.y + b.y) / 2); }
float3 MovePoint(const float3 a, const float3 b, const float distance) { return a + (normalize(b - a) * distance); }
float2 MovePoint(const float2 a, const float2 b, const float distance) { return a + (normalize(b - a) * distance); }
float3 MovePoint01(const float3 a, const float3 b, const float distance) { return lerp(a, b, distance); }
float2 MovePoint01(const float2 a, const float2 b, const float distance) { return lerp(a, b, distance); }

bool IsPrime(const int num)
{
    if (num == 1) { return true; }

    int sqrtNum = sqrt(num);
    for (int i = 2; i <= sqrtNum; i++) 
    {
        if (num % i == 0) { return false; }
    }
 
    return true;
}

float pow(const float a, const float b) 
{
    if (floor(b) == ceil(b)) 
    {
        if (b == 0) { return 1; }
        float result = 1;
        uint16 i = abs(b);
        while (i > 0) { result *= a; i--; }
        return (b < 0) ? 1 / result : result;
    }

    if (a < 0) { return 0; }
    int p = floor(abs(b));
    float result = pow(a, p);
    if (abs(b) - p > 0.3 && abs(b) - p < 0.7) { result *= sqrt(a); }
    return (b < 0) ? 1 / result : result;
}

#endif