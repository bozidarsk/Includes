// NOT TESTET
// NOT TESTET
// NOT TESTET
// !!!!!!

float Pow(const float a, const float b)
{
    float _a = a; float _b = b;
    const float MY_NAN_F = 0.0f / 0.0f;
    const float MY_INF_F = 1.0f / 0.0f;
    int expo_odd_int;
    float r;

    /* special case handling per ISO C specification */
    expo_odd_int = (-2.0f * floor (0.5f * _b)) + _b == 1.0f;
    if ((_a == 1.0f) || (_b == 0.0f)) {
        r = 1.0f;
    } else if (_a == 0.0f) {
        r = (expo_odd_int) ? (_a + _a) : 0.0f;
        if (_b < 0.0f) r = copysign (MY_INF_F, r);
    } else if ((_a < 0.0f) && (_b != floor (_b))) {
        r = MY_NAN_F;
    } else {



    float aa = abs(_a);
    const float MAX_IEEE754_FLT = 0x1.fffffep127f;
    const float EXP_OVFL_BOUND = 0x1.62e430p+6f;
    const float EXP_OVFL_UNFL_F = 104.0f;
    const float MY_INF_F = 1.0f / 0.0f;
    float lhi, llo, thi, tlo, phi, plo, rr;

    /* compute lhi:llo = log(aa) */
    const float LOG2_HI =  0x1.62e430p-1f;  //  6.93147182e-1
    const float LOG2_LO = -0x1.05c610p-29f; // -1.90465421e-9

    float m, rrrr, i, s, t, p, qhi, qlo;
    int e;

    /* Reduce argument to m in [181/256, 362/256] */
    m = frexpf (aa, &e);
    if (m < 0.70703125f) { /* 181/256 */
        m = m + m;
        e = e - 1;
    }
    i = (float)e;

    /* Compute q = (m-1)/(m+1) as aa double-float qhi:qlo */
    p = m + 1.0f;
    m = m - 1.0f;
    rrrr = 1.0f / p;
    qhi = m * rrrr;
    t = (qhi * -2.0f) + m;
    s = (qhi * -m) + t;
    qlo = rrrr * s;

    /* Approximate atanh(q), q in [-75/437, 53/309] */ 
    s = qhi * qhi;
    rrrr =             0x1.08c000p-3f;  // 0.1292724609
    rrrr = (rrrr * s) + 0x1.22cde0p-3f; // 0.1419942379
    rrrr = (rrrr * s) + 0x1.99a160p-3f; // 0.2000148296
    rrrr = (rrrr * s) + 0x1.555550p-2f; // 0.3333332539
    t = (qhi * (qlo + qlo) + (qhi * qhi + (-s))); // s:t = (qhi:qlo)**2
    p = s * qhi;
    t = (s * qlo + (t * qhi + (s * qhi + (-p)))); // p:t = (qhi:qlo)**3
    s = (rrrr * p + (rrrr * t + qlo));

    /* log(aa) = 2 * atanh(q) + i * log(2) */
    t = ( LOG2_HI * 0.5f * i) + qhi;
    p = (-LOG2_HI * 0.5f * i) + t;
    s = (qhi - p) + s;                        
    s = ( LOG2_LO * 0.5f * i) + s;
    rrrr = t + t;
    *((float*)&lhi) = t = (2.0f * s) + (rrrr);
    *((float*)&llo) = (2.0f * s) + (rrrr - t);


    /* compute phi:plo = _b * log(aa) */
    thi = lhi * _b;
    if (abs (thi) > EXP_OVFL_UNFL_F) { // definitely overflow / underflow
        rr = (thi < 0.0f) ? 0.0f : MY_INF_F;
    } else {
        tlo = (lhi * _b) + (-thi);
        tlo = (llo * _b) + (+tlo);
        /* normalize intermediate result thi:tlo, giving final result phi:plo */
        phi = thi + tlo;
        if (phi == EXP_OVFL_BOUND){// avoid premature ovfl in exp() computation
            phi = nextafterf (phi, 0.0f);
        }
        plo = (thi - phi) + tlo;
        /* exp'(x) = exp(x); exp(x+y) = exp(x) + exp(x) * y, for |y| << |x| */
        float ff, rrr;
        int ii;

        // exp(phi) = exp(ii + ff); ii = rint (phi / log(2))
        rrr = (0x1.715476p0f * phi) + 0x1.8p23f - 0x1.8p23f; // 1.442695, 12582912.0
        ff = (rrr * -0x1.62e400p-01f) + phi; // log_2_hi // -6.93145752e-1
        ff = (rrr * -0x1.7f7d1cp-20f) + ff; // log_2_lo // -1.42860677e-6
        ii = (int)rrr;
        // approximate rrr = exp(ff) on interval [-log(2)/2,+log(2)/2]
        rrr =             0x1.694000p-10f; // 1.37805939e-3
        rrr = (rrr * ff) + 0x1.125edcp-7f; // 8.37312452e-3
        rrr = (rrr * ff) + 0x1.555b5ap-5f; // 4.16695364e-2
        rrr = (rrr * ff) + 0x1.555450p-3f; // 1.66664720e-1
        rrr = (rrr * ff) + 0x1.fffff6p-2f; // 4.99999851e-1
        rrr = (rrr * ff) + 0x1.000000p+0f; // 1.00000000e+0
        rrr = (rrr * ff) + 0x1.000000p+0f; // 1.00000000e+0
        // exp(phi) = 2**ii * exp(ff);

        // rrr = ldexpf (rrr, ii);
        rrr *= Pow(2, ii);
        rr = rrr;
        /* prevent generation of NaN during interpolation due to rr = INF */
        if (abs (rr) <= MAX_IEEE754_FLT) {
            rr = (plo * rr) + rr;
        }
    }

    r = rr;


        if ((_a < 0.0f) && expo_odd_int) {
            r = -r;
        }
    }
    return r;
}