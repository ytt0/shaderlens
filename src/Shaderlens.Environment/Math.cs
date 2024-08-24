namespace Shaderlens
{
    // https://registry.khronos.org/OpenGL/specs/gl/GLSLangSpec.4.40.pdf

    public static partial class Environment
    {
        public static bool _bool(int v) { return default; }
        public static bool _bool(uint v) { return default; }
        public static bool _bool(Float v) { return default; }
        public static bool bool_(int v) { return default; }
        public static bool bool_(uint v) { return default; }
        public static bool bool_(Float v) { return default; }

        public static int _int(bool v) { return default; }
        public static int _int(uint v) { return default; }
        public static int _int(Float v) { return default; }
        public static int int_(bool v) { return default; }
        public static int int_(uint v) { return default; }
        public static int int_(Float v) { return default; }

        public static int _uint(bool v) { return default; }
        public static int _uint(int v) { return default; }
        public static int _uint(Float v) { return default; }
        public static int uint_(bool v) { return default; }
        public static int uint_(int v) { return default; }
        public static int uint_(Float v) { return default; }

        public static Float _float(bool v) { return default; }
        public static Float _float(int v) { return default; }
        public static Float _float(uint v) { return default; }
        public static Float float_(bool v) { return default; }
        public static Float float_(int v) { return default; }
        public static Float float_(uint v) { return default; }

        public static int length<T>(this T[] array) { return default; }

        #region 8.5 Geometric Functions
        public static vec3 cross(vec3 x, vec3 y) { return default; }
        // Available only when using the compatibility profile. For core OpenGL, use invariant.
        public static vec4 ftransform() { return default; }
        #endregion

        #region 8.6 Matrix Functions
        // Multiply matrix x by matrix y component-wise, i.e., result[i][j] is the scalar product of x[i][j] and y[i][j]. Note: to get linear algebraic matrix multiplication, use the multiply operator(*).
        public static mat2 outerProduct(vec2 c, vec2 r) { return default; }
        //
        public static mat3 outerProduct(vec3 c, vec3 r) { return default; }
        //
        public static mat4 outerProduct(vec4 c, vec4 r) { return default; }
        ////
        //public static mat2x3 outerProduct(vec3 c, vec2 r) { return default; }
        ////
        //public static mat3x2 outerProduct(vec2 c, vec3 r) { return default; }
        ////
        //public static mat2x4 outerProduct(vec4 c, vec2 r) { return default; }
        ////
        //public static mat4x2 outerProduct(vec2 c, vec4 r) { return default; }
        ////
        //public static mat3x4 outerProduct(vec4 c, vec3 r) { return default; }
        //// Treats the first parameter c as a column vector(matrix with one column) and the second parameter r as a row vector(matrix with one row) and does a linear algebraic matrix multiply c * r, yielding a matrix whose number of rows is the number of components in c and whose number of columns is the number of components in r.
        //public static mat4x3 outerProduct(vec3 c, vec4 r) { return default; }
        //
        public static mat2 transpose(mat2 m) { return default; }
        //
        public static mat3 transpose(mat3 m) { return default; }
        //
        public static mat4 transpose(mat4 m) { return default; }
        ////
        //public static mat2x3 transpose(mat3x2 m) { return default; }
        ////
        //public static mat3x2 transpose(mat2x3 m) { return default; }
        ////
        //public static mat2x4 transpose(mat4x2 m) { return default; }
        ////
        //public static mat4x2 transpose(mat2x4 m) { return default; }
        ////
        //public static mat3x4 transpose(mat4x3 m) { return default; }
        //// Returns a matrix that is the transpose of m. The input matrix m is not modified.
        //public static mat4x3 transpose(mat3x4 m) { return default; }
        //
        public static Float determinant(mat2 m) { return default; }
        //
        public static Float determinant(mat3 m) { return default; }
        // Returns the determinant of m.
        public static Float determinant(mat4 m) { return default; }
        //
        public static mat2 inverse(mat2 m) { return default; }
        //
        public static mat3 inverse(mat3 m) { return default; }
        // Returns a matrix that is the inverse of m. The input matrix m is not modified. The values in the returned matrix are undefined if m is singular or poorly- conditioned(nearly singular).
        public static mat4 inverse(mat4 m) { return default; }
        #endregion

        #region 8.7 Vector Relational Functions
        ////
        //public static bvec lessThan(vec x, vec y) { return default; }
        ////
        //public static bvec lessThan(ivec x, ivec y) { return default; }
        //// Returns the component-wise compare of x < y.
        //public static bvec lessThan(uvec x, uvec y) { return default; }
        ////
        //public static bvec lessThanEqual(vec x, vec y) { return default; }
        ////
        //public static bvec lessThanEqual(ivec x, ivec y) { return default; }
        //// Returns the component-wise compare of x <= y.
        //public static bvec lessThanEqual(uvec x, uvec y) { return default; }
        ////
        //public static bvec greaterThan(vec x, vec y) { return default; }
        ////
        //public static bvec greaterThan(ivec x, ivec y) { return default; }
        //// Returns the component-wise compare of x > y.
        //public static bvec greaterThan(uvec x, uvec y) { return default; }
        ////
        //public static bvec greaterThanEqual(vec x, vec y) { return default; }
        ////
        //public static bvec greaterThanEqual(ivec x, ivec y) { return default; }
        //// Returns the component-wise compare of x >= y.
        //public static bvec greaterThanEqual(uvec x, uvec y) { return default; }
        ////
        //public static bvec equal(vec x, vec y) { return default; }
        ////
        //public static bvec equal(ivec x, ivec y) { return default; }
        ////
        //public static bvec equal(uvec x, uvec y) { return default; }
        ////
        //public static bvec equal(bvec x, bvec y) { return default; }
        ////
        //public static bvec notEqual(vec x, vec y) { return default; }
        ////
        //public static bvec notEqual(ivec x, ivec y) { return default; }
        ////
        //public static bvec notEqual(uvec x, uvec y) { return default; }
        //// Returns the component-wise compare of x == y. Returns the component-wise compare of x != y.
        //public static bvec notEqual(bvec x, bvec y) { return default; }
        //// Returns true if any component of x is true.
        //public static bool any(bvec x) { return default; }
        //// Returns true only if all components of x are true.
        //public static bool all(bvec x) { return default; }
        //// Returns the component-wise logical complement of x.
        //public static bvec not(bvec x) { return default; }
        #endregion
    }

    public static partial class Environment<genType>
    {
        #region 8.1 Angle and Trigonometry Functions
        // Converts degrees to radians, i.e.,  180 ⋅degrees
        public static genType radians(genType degrees) { return default; }
        // Converts radians to degrees, i.e., 180  ⋅radians
        public static genType degrees(genType radians) { return default; }
        // The standard trigonometric sine function.
        public static genType sin(genType angle) { return default; }
        // The standard trigonometric cosine function.
        public static genType cos(genType angle) { return default; }
        // The standard trigonometric tangent.
        public static genType tan(genType angle) { return default; }
        // Arc sine. Returns an angle whose sine is x. The range of values returned by this function is [− 2 , 2 ] Results are undefined if ∣x∣1. genType acos(genType x)
        public static genType asin(genType x) { return default; }
        // Arc cosine. Returns an angle whose cosine is x. The range of values returned by this function is [0, π]. Results are undefined if ∣x∣1.
        public static genType acos(genType x) { return default; }
        // Arc tangent. Returns an angle whose tangent is y/x. The signs of x and y are used to determine what quadrant the angle is in. The range of values returned by this function is [− , ]. Results are undefined if x and y are both 0.
        public static genType atan(genType y, genType x) { return default; }
        // Arc tangent. Returns an angle whose tangent is y_over_x. The range of values returned by this function is [−  2 ,  2 ]
        public static genType atan(genType y_over_x) { return default; }
        // Returns the hyperbolic sine function ex−e−x 2
        public static genType sinh(genType x) { return default; }
        // Returns the hyperbolic cosine function exe−x 2
        public static genType cosh(genType x) { return default; }
        // Returns the hyperbolic tangent function sinh x cosh  x
        public static genType tanh(genType x) { return default; }
        // Arc hyperbolic sine; returns the inverse of sinh.
        public static genType asinh(genType x) { return default; }
        // Arc hyperbolic cosine; returns the non-negative inverse of cosh. Results are undefined if x < 1.
        public static genType acosh(genType x) { return default; }
        // Arc hyperbolic tangent; returns the inverse of tanh. Results are undefined if ∣x∣≥1.
        public static genType atanh(genType x) { return default; }
        #endregion

        #region 8.2 Exponential Functions
        // Returns x raised to the y power, i.e., xy Results are undefined if x < 0. Results are undefined if x = 0 and y <= 0.
        public static genType pow(genType x, genType y) { return default; }
        // Returns the natural exponentiation of x, i.e., ex.
        public static genType exp(genType x) { return default; }
        // Returns the natural logarithm of x, i.e., returns the value y which satisfies the equation x = ey. Results are undefined if x <= 0.
        public static genType log(genType x) { return default; }
        // Returns 2 raised to the x power, i.e., 2x
        public static genType exp2(genType x) { return default; }
        // Returns the @base 2 logarithm of x, i.e., returns the value y which satisfies the equation x= 2y Results are undefined if x <= 0.
        public static genType log2(genType x) { return default; }
        // Returns √ x . Results are undefined if x < 0.
        public static genType sqrt(genType x) { return default; }
        // Returns 1 √ x . Results are undefined if x <= 0.
        public static genType inversesqrt(genType x) { return default; }
        #endregion

        #region 8.3 Common Functions
        // Returns x if x >= 0; otherwise it returns –x.
        public static genType abs(genType x) { return default; }
        // Returns 1.0 if x > 0, 0.0 if x = 0, or –1.0 if x < 0.
        public static genType sign(genType x) { return default; }
        // Returns a value equal to the nearest integer that is less than or equal to x.
        public static genType floor(genType x) { return default; }
        // Returns a value equal to the nearest integer to x whose absolute value is not larger than the absolute value of x.
        public static genType trunc(genType x) { return default; }
        // Returns a value equal to the nearest integer to x. The fraction 0.5 will round in a direction chosen by the implementation, presumably the direction that is fastest. This includes the possibility that round(x) returns the same value as roundEven(x) for all values of x.
        public static genType round(genType x) { return default; }
        // Returns a value equal to the nearest integer to x. A fractional part of 0.5 will round toward the nearest even integer. (Both 3.5 and 4.5 for x will return 4.0.)
        public static genType roundEven(genType x) { return default; }
        // Returns a value equal to the nearest integer that is greater than or equal to x.
        public static genType ceil(genType x) { return default; }
        // Returns x – floor (x)
        public static genType fract(genType x) { return default; }
        // -
        public static genType mod(genType x, Float y) { return default; }
        // Modulus. Returns x – y ∗ floor (x/y).
        public static genType mod(genType x, genType y) { return default; }
        // Returns the fractional part of x and sets i to the integer part (as a whole number doubleing-point value). Both the return value and the output parameter will have the same sign as x.
        public static genType modf(genType x, out genType i) { i = default; return default; }
        // -
        public static genType min(genType x, genType y) { return default; }
        // -
        public static genType min(genType x, Float y) { return default; }
        // -
        public static genType min(genType x, int y) { return default; }
        // Returns y if y < x; otherwise it returns x.
        public static genType min(genType x, uint y) { return default; }
        // -
        public static genType max(genType x, genType y) { return default; }
        // -
        public static genType max(genType x, Float y) { return default; }
        // -
        public static genType max(genType x, int y) { return default; }
        // Returns y if x < y; otherwise it returns x.
        public static genType max(genType x, uint y) { return default; }
        // -
        public static genType clamp(genType x, genType minVal, genType maxVal) { return default; }
        // -
        public static genType clamp(genType x, Float minVal, Float maxVal) { return default; }
        // -
        public static genType clamp(genType x, int minVal, int maxVal) { return default; }
        //  Returns min (max (x, minVal), maxVal). Results are undefined if minVal > maxVal.
        public static genType clamp(genType x, uint minVal, uint maxVal) { return default; }
        // Returns the linear blend of x and y, i.e., x⋅(1−a ) + y⋅a
        public static genType mix(genType x, genType y, Float a) { return default; }
        // Selects which vector each returned component comes from. For a component of a that is false, the corresponding component of x is returned. For a component of a that is true, the corresponding component of y is returned. Components of x and y that are not selected are allowed to be invalid doubleing-point values and will have no effect on the results. Thus, this provides different functionality than, for example,
        public static genType mix(genType x, genType y, genType a) { return default; }
        // -
        public static genType step(genType edge, genType x) { return default; }
        // Returns 0.0 if x < edge; otherwise it returns 1.0.
        public static genType step(Float edge, genType x) { return default; }
        // -
        public static genType smoothstep(genType edge0, genType edge1, genType x) { return default; }
        // Returns 0.0 if x <= edge0 and 1.0 if x >= edge1 and performs smooth Hermite interpolation between 0 and 1 when edge0 < x < edge1. This is useful in cases where you would want a threshold function with a smooth transition. This is equivalent to: genType t; t = clamp ((x – edge0) / (edge1 – edge0), 0, 1); return t * t * (3 – 2 * t); (And similarly for doubles.) Results are undefined if edge0 >= edge1.
        public static genType smoothstep(Float edge0, Float edge1, genType x) { return default; }
        // Returns true if x holds a NaN. Returns false otherwise. Always returns false if NaNs are not implemented.
        public static genType isnan(genType x) { return default; }
        // Returns true if x holds a positive infinity or negative infinity. Returns false otherwise.
        public static genType isinf(genType x) { return default; }
        // -
        public static genType doubleBitsToInt(genType value) { return default; }
        // Returns a signed or unsigned integer value representing the encoding of a Float. The Float value's bit-level representation is preserved.
        public static genType doubleBitsToUint(genType value) { return default; }
        // -
        public static genType intBitsTodouble(genType value) { return default; }
        // Returns a Float value corresponding to a signed or unsigned integer encoding of a Float. If a NaN is passed in, it will not signal, and the resulting value is unspecified. If an Inf is passed in, the resulting value is the corresponding Inf.
        public static genType uintBitsTodouble(genType value) { return default; }
        // Computes and returns a*b + c. In uses where the return value is eventually consumed by a variable declared as precise: • fma() is considered a single operation, whereas the expression “a*b + c” consumed by a variable declared precise is considered two operations. • The precision of fma() can differ from the precision of the expression “a*b + c”. • fma() will be computed with the same precision as any other fma() consumed by a precise variable, giving invariant results for the same input values of a, b, and c. Otherwise, in the absence of precise consumption, there are no special constraints on the number of operations or difference in precision between fma() and the expression “a*b + c”.
        public static genType fma(genType a, genType b, genType c) { return default; }
        // Splits x into a doubleing-point significand in the range [0.5, 1.0) and an integral exponent of two, such that: x= significand⋅2exponent The significand is returned by the function and the exponent is returned in the parameter exp. For a doubleing-point value of zero, the significand and exponent are both zero. For a doubleing-point value that is an infinity or is not a number, the results are undefined. If an implementation supports negative 0, frexp(-0) should return -0; otherwise it will return 0.
        public static genType frexp(genType x, out genType exp) { exp = default; return default; }
        // Builds a floating-point number from x and the corresponding integral exponent of two in exp, returning: significand⋅2exponent If this product is too large to be represented in the floating-point type, the result is undefined.
        public static genType ldexp(genType x, in genType exp) { return default; }
        #endregion

        #region 8.5 Geometric Functions
        // Returns the length of vector x, i.e., √ x[0]2+x[1]2+...
        public static Float length(genType x) { return default; }
        // Returns the distance between p0 and p1, i.e., length (p0 – p1)
        public static Float distance(genType p0, genType p1) { return default; }
        // Returns the dot product of x and y, i.e., x[0]⋅y [0]+ x[1]⋅y [1]+...
        public static Float dot(genType x, genType y) { return default; }
        // Returns a vector in the same direction as x but with a length of 1. compatibility profile only
        public static genType normalize(genType x) { return default; }

        // If dot(Nref, I) < 0 return N, otherwise return –N.
        public static genType faceforward(genType N, genType I, genType Nref) { return default; }
        // For the incident vector I and surface orientation N, returns the reflection direction: I – 2 ∗ dot(N, I) ∗ N N must already be normalized in order to achieve the desired result.
        public static genType reflect(genType I, genType N) { return default; }
        //For the incident vector I and surface normal N, and the ratio of indices of refraction eta, return the refraction vector. The result is computed by k = 1.0 - eta * eta *(1.0 - dot(N, I) * dot(N, I)) if(k < 0.0) return genType(0.0) // or genType(0.0) else return eta * I -(eta * dot(N, I) + sqrt(k)) * N The input parameters for the incident vector I and the surface normal N must already be normalized to get the desired results.
        public static genType refract(genType I, genType N, Float eta) { return default; }
        #endregion

        #region 8.8 Integer Functions
        // Adds 32-bit unsigned integer x and y, returning the sum modulo 232. The value carry is set to 0 if the sum was less than 232, or to 1 otherwise.
        public static genType uaddCarry(genType x, genType y, out genType carry) { carry = default; return default; }
        // Subtracts the 32-bit unsigned integer y from x, returning the difference if non-negative, or 232 plus the difference otherwise. The value borrow is set to 0 if x >= y, or to 1 otherwise.
        public static genType usubBorrow(genType x, genType y, out genType borrow) { borrow = default; return default; }
        //
        public static void umulExtended(genType x, genType y, out genType msb, out genType lsb) { msb = default; lsb = default; }
        // Multiplies 32-bit integers x and y, producing a 64-bit result. The 32 least-significant bits are returned in lsb. The 32 most-significant bits are returned in msb.
        public static void imulExtended(genType x, genType y, out genType msb, out genType lsb) { msb = default; lsb = default; }
        // Extracts bits [offset, offset + bits - 1] from value, returning them in the least significant bits of the result. For unsigned data types, the most significant bits of the result will be set to zero. For signed data types, the most significant bits will be set to the value of bit offset + bits – 1. If bits is zero, the result will be zero. The result will be undefined if offset or bits is negative, or if the sum of offset and bits is greater than the number of bits used to store the operand.
        public static genType bitfieldExtract(genType value, int offset, int bits) { return default; }
        // Returns the insertion of the bits least-significant bits of insert into base. The result will have bits [offset, offset + bits - 1] taken from bits [0, bits – 1] of insert, and all other bits taken directly from the corresponding bits of @base. If bits is zero, the result will simply be @base. The result will be undefined if offset or bits is negative, or if the sum of offset and bits is greater than the number of bits used to store the operand.
        public static genType bitfieldInsert(genType @base, genType insert, int offset, int bits) { return default; }
        // Returns the reversal of the bits of value. The bit numbered n of the result will be taken from bit(bits - 1) - n of value, where bits is the total number of bits used to represent value.
        public static genType bitfieldReverse(genType value) { return default; }
        // Returns the number of bits set to 1 in the binary representation of value.
        public static genType bitCount(genType value) { return default; }
        // Returns the bit number of the least significant bit set to 1 in the binary representation of value. If value is zero, -1will be returned.
        public static genType findLSB(genType value) { return default; }
        // Returns the bit number of the most significant bit in the binary representation of value. For positive integers, the result will be the bit number of the most significant bit set to 1. For negative integers, the result will be the bit number of the most significant bit set to 0. For a value of zero or negative one, -1 will be returned.genType findMSB(genType value)
        public static genType findMSB(genType value) { return default; }
        #endregion
    }
}
