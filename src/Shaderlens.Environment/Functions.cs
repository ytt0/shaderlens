namespace Shaderlens
{
    // Reference: https://registry.khronos.org/OpenGL/specs/gl/GLSLangSpec.4.60.pdf

    public static partial class Environment
    {
        #region 8.3. Common Functions

        // Returns the fractional part of x and sets i to the integer part (as a whole number floating-point value). Both the return value and the output parameter will have the same sign as x.
        public static double modf(double x, out double i) { i = default; return default; }

        #endregion

        #region  8.4. Floating-Point Pack and Unpack Functions

        // First, converts each component of the normalized floating-point value v into 16-bit (2x16) or 8-bit (4x8) integer values. Then, the results are packed into the returned 32-bit unsigned integer.
        // The conversion for component c of v to fixed point is done as follows:
        // packUnorm2x16: round(clamp(c, 0, +1) * 65535.0).
        // packSnorm2x16: round(clamp(c, -1, +1) * 32767.0).
        // packUnorm4x8: round(clamp(c, 0, +1) * 255.0).
        // packSnorm4x8: round(clamp(c, -1, +1) * 127.0).
        // The first component of the vector will be written to the least significant bits of the output; the last component will be  written to the most significant bits.
        public static uint packUnorm2x16(vec2 v) { return default; }
        public static uint packSnorm2x16(vec2 v) { return default; }
        public static uint packUnorm4x8(vec4 v) { return default; }
        public static uint packSnorm4x8(vec4 v) { return default; }

        // First, unpacks a single 32-bit unsigned integer p into a pair of 16-bit unsigned integers, a pair of 16-bit signed integers, four 8-bit unsigned integers, or four 8-bit signed integers, respectively. Then, each component is converted to a normalized floating-point value to generate the returned two- or four-component vector.
        // The conversion for unpacked fixed-point value f to floating-point is done as follows:
        // unpackUnorm2x16: f / 65535.0
        // unpackSnorm2x16: clamp(f / 32767.0, -1, +1)
        // unpackUnorm4x8: f / 255.0
        // unpackSnorm4x8: clamp(f / 127.0, -1, +1)
        // The first component of the returned vector will be extracted from the least significant bits of the input; the last component will be extracted from the most significant bits.
        public static vec2 unpackUnorm2x16(uint p) { return default; }
        public static vec2 unpackSnorm2x16(uint p) { return default; }
        public static vec4 unpackUnorm4x8(uint p) { return default; }
        public static vec4 unpackSnorm4x8(uint p) { return default; }

        // Returns an unsigned integer obtained by converting the components of a two-component floating-point vector to the 16-bit floating-point representation of the API, and then packing these two 16-bit integers into a 32-bit unsigned integer.
        // The first vector component specifies the 16 least-significant bits of the result; the second component specifies the 16 most-significant bits.
        public static uint packHalf2x16(vec2 v) { return default; }

        // Returns a two-component floating-point vector with components obtained by unpacking a 32-bit unsigned integer into a pair of 16-bit values, interpreting those values as 16-bit floating-point numbers according to the API, and converting them to 32-bit floating-point values.
        // The first component of the vector is obtained from the 16 least-significant bits of v; the second component is obtained from the 16 most-significant bits of v.
        public static vec2 unpackHalf2x16(uint v) { return default; }

        // Returns a double-precision value obtained by packing the components of v into a 64-bit value. If an IEEE 754 Inf or NaN is created, it will not signal, and the resulting floating-point value is unspecified. Otherwise, the bit-level representation of v is preserved. The first vector component specifies the 32 least significant bits; the second component specifies the 32 most significant bits.
        public static double packDouble2x32(uvec2 v) { return default; }

        // Returns a two-component unsigned integer vector representation of v. The bit-level representation of v is preserved. The first component of the vector contains the 32 least significant bits of the double; the second component consists of the 32 most significant bits.
        public static uvec2 unpackDouble2x32(double v) { return default; }

        #endregion

        #region 8.5. Geometric Functions

        // Returns the cross product of x and y, i.e., (x1 · y2 - y1 · x2, x2 · y0 - y2 · x0, x0 · y1 - y0 · x1).
        public static vec3 cross(vec3 x, vec3 y) { return default; }
        public static dvec3 cross(dvec3 x, dvec3 y) { return default; }

        // Available only when using the compatibility profile. For core OpenGL, use invariant. For vertex shaders only. This function will ensure that the incoming vertex value will be transformed in a way that produces exactly the same result as would be produced by OpenGL’s fixed functionality transform. It is intended to be used to compute gl_Position, e.g.
        // gl_Position = ftransform()
        // This function should be used, for example, when an application is rendering the same geometry in separate passes, and one pass uses the fixed functionality path to render and another pass uses programmable shaders.
        public static vec4 ftransform() { return default; }

        #endregion

        #region 8.6. Matrix Functions

        // Multiply matrix x by matrix y component-wise, i.e., result[i][j] is the scalar product of x[i][j] and y[i][j].
        // Note: to get linear algebraic matrix multiplication, use the multiply operator (*).
        public static mat2 matrixCompMult(mat2 x, mat2 y) { return default; }
        public static mat3 matrixCompMult(mat3 x, mat3 y) { return default; }
        public static mat4 matrixCompMult(mat4 x, mat4 y) { return default; }
        public static mat2x3 matrixCompMult(mat2x3 x, mat2x3 y) { return default; }
        public static mat3x2 matrixCompMult(mat3x2 x, mat3x2 y) { return default; }
        public static mat2x4 matrixCompMult(mat2x4 x, mat2x4 y) { return default; }
        public static mat4x2 matrixCompMult(mat4x2 x, mat4x2 y) { return default; }
        public static mat3x4 matrixCompMult(mat3x4 x, mat3x4 y) { return default; }
        public static mat4x3 matrixCompMult(mat4x3 x, mat4x3 y) { return default; }

        // Treats the first parameter c as a column vector (matrix with one column) and the second parameter r as a row vector (matrix with one row) and does a linear algebraic matrix multiply c * r, yielding a matrix whose number of rows is the number of components in c and whose number of columns is the number of components in r.
        public static mat2 outerProduct(vec2 c, vec2 r) { return default; }
        public static mat3 outerProduct(vec3 c, vec3 r) { return default; }
        public static mat4 outerProduct(vec4 c, vec4 r) { return default; }
        public static mat2x3 outerProduct(vec3 c, vec2 r) { return default; }
        public static mat3x2 outerProduct(vec2 c, vec3 r) { return default; }
        public static mat2x4 outerProduct(vec4 c, vec2 r) { return default; }
        public static mat4x2 outerProduct(vec2 c, vec4 r) { return default; }
        public static mat3x4 outerProduct(vec4 c, vec3 r) { return default; }
        public static mat4x3 outerProduct(vec3 c, vec4 r) { return default; }

        // Returns a matrix that is the transpose of m. The input matrix m is not modified.
        public static mat2 transpose(mat2 m) { return default; }
        public static mat3 transpose(mat3 m) { return default; }
        public static mat4 transpose(mat4 m) { return default; }
        public static mat2x3 transpose(mat3x2 m) { return default; }
        public static mat3x2 transpose(mat2x3 m) { return default; }
        public static mat2x4 transpose(mat4x2 m) { return default; }
        public static mat4x2 transpose(mat2x4 m) { return default; }
        public static mat3x4 transpose(mat4x3 m) { return default; }
        public static mat4x3 transpose(mat3x4 m) { return default; }

        // Returns the determinant of m.
        public static Float determinant(mat2 m) { return default; }
        public static Float determinant(mat3 m) { return default; }
        public static Float determinant(mat4 m) { return default; }

        // Returns a matrix that is the inverse of m. The input matrix m is not modified. The values in the returned matrix are undefined if m is singular or poorly-conditioned (nearly singular).
        public static mat2 inverse(mat2 m) { return default; }
        public static mat3 inverse(mat3 m) { return default; }
        public static mat4 inverse(mat4 m) { return default; }

        #endregion

        #region 8.9.1 Texture Functions (sampler2D only)

        // Returns the dimensions of level lod (if present) for the texture bound to sampler, as described in section 11.1.3.4 “Texture Queries” of the OpenGL Specification.
        // The components in the return value are filled in, in order, with the width, height, and depth of the texture.
        // For the array forms, the last component of the return value is the number of layers in the texture array, or the number of cubes in the texture cube map array.
        public static ivec2 textureSize(sampler2D sampler, int lod) { return default; }

        // Returns the mipmap array(s) that would be accessed in the x component of the return value. Returns the computed level-of-detail relative to the base level in the y component of the return value.
        // If called on an incomplete texture, the results are undefined.
        public static vec2 textureQueryLod(sampler2D sampler, vec2 P) { return default; }

        // Returns the number of mipmap levels accessible in the texture associated with sampler, as defined in the OpenGL Specification.
        // The value zero will be returned if no texture or an incomplete texture is associated with sampler.
        // Available in all shader stages.
        public static int textureQueryLevels(sampler2D sampler) { return default; }

        #endregion

        #region 8.9.2. Texel Lookup Functions (sampler2D only)(1)

        // Use the texture coordinate P to do a texture lookup in the texture currently bound to sampler.
        // For shadow forms: When compare is present, it is used as Dref and the array layer comes from the last component of P. When compare is not present, the last component of P is used as Dref and the array layer comes from the second to last component of P. (The second component of P is unused for 1D shadow lookups.)
        // For non-shadow forms: the array layer comes from the last component of P.
        public static vec4 texture(sampler2D sampler, vec2 P, Float bias = default) { return default; }

        // Do a texture lookup with projection. The texture coordinates consumed from P, not including the last component of P, are divided by the last component of P to form projected coordinates P'.
        // The resulting third component of P in the shadow forms is used as Dref. The third component of P is ignored when sampler has type sampler2D and P has type vec4. After these values are computed, texture lookup proceeds as in texture.
        public static vec4 textureProj(sampler2D sampler, vec3 P, Float bias = default) { return default; }

        // Do a texture lookup as in texture but with explicit level-of-detail; lod specifies λbase] and sets the partial derivatives as follows:
        // (See section 8.14 “Texture Minification” and equations 8.4-8.6 of the OpenGL Specification.)
        // ∂u / ∂x = ∂v / ∂x = ∂w / ∂x = 0
        // ∂u / ∂y = ∂v / ∂y = ∂w / ∂y = 0
        public static vec4 textureLod(sampler2D sampler, vec2 P, Float lod) { return default; }

        // Do a texture lookup as in texture but with offset added to the (u,v,w) texel coordinates before looking up each texel. The offset value must be a constant expression. A limited range of offset values are supported; the minimum and maximum offset values are implementation-dependent and given by gl_MinProgramTexelOffset and gl_MaxProgramTexelOffset, respectively.
        // Note that offset does not apply to the layer coordinate for texture arrays. This is explained in detail in section 8.14.2 “Coordinate Wrapping and Texel Selection” of the OpenGL Specification, where offset is (δu, δv, δw).
        // Note that texel offsets are also not supported for cube maps.
        public static vec4 textureOffset(sampler2D sampler, vec2 P, ivec2 offset, Float bias = default) { return default; }

        // Use integer texture coordinate P to lookup a single texel from sampler. The array layer comes from the last component of P for the array forms. The level-of-detail lod (if present) is as described in sections 11.1.3.2 “Texel Fetches” and 8.14.1 “Scale Factor and Level of Detail” of the OpenGL Specification.
        public static vec4 texelFetch(sampler2D sampler, ivec2 P, int lod) { return default; }

        // Fetch a single texel as in texelFetch, offset by offset as described in textureOffset.
        public static vec4 texelFetchOffset(sampler2D sampler, ivec2 P, int lod, ivec2 offset) { return default; }

        // Do a projective texture lookup as described in textureProj, offset by offset as described in textureOffset.
        public static vec4 textureProjOffset(sampler2D sampler, vec3 P, ivec2 offset, Float bias = default) { return default; }

        // Do an offset texture lookup with explicit level-of- detail. See textureLod and textureOffset.
        public static vec4 textureLodOffset(sampler2D sampler, vec2 P, Float lod, ivec2 offset) { return default; }

        // Do a projective texture lookup with explicit level-of-detail. See textureProj and textureLod.
        public static vec4 textureProjLod(sampler2D sampler, vec3 P, Float lod) { return default; }
        public static vec4 textureProjLod(sampler2D sampler, vec4 P, Float lod) { return default; }

        // Do an offset projective texture lookup with explicit level-of-detail. See textureProj, textureLod, and textureOffset.
        public static vec4 textureProjLodOffset(sampler2D sampler, vec3 P, Float lod, ivec2 offset) { return default; }
        public static vec4 textureProjLodOffset(sampler2D sampler, vec4 P, Float lod, ivec2 offset) { return default; }

        // Do a texture lookup as in texture but with explicit gradients as shown below. The partial derivatives of P are with respect to window x and window y. For the cube version, the partial derivatives of P are assumed to be in the coordinate system used before texture coordinates are projected onto the appropriate cube face.
        public static vec4 textureGrad(sampler2D sampler, vec2 P, vec2 dPdx, vec2 dPdy) { return default; }

        // Do a texture lookup with both explicit gradient and offset, as described in textureGrad and textureOffset.
        public static vec4 textureGradOffset(sampler2D sampler, vec2 P, vec2 dPdx, vec2 dPdy, ivec2 offset) { return default; }

        // Do a texture lookup both projectively, as described in textureProj, and with explicit gradient as described in textureGrad. The partial derivatives dPdx and dPdy are assumed to be already projected.
        public static vec4 textureProjGrad(sampler2D sampler, vec3 P, vec2 dPdx, vec2 dPdy) { return default; }
        public static vec4 textureProjGrad(sampler2D sampler, vec4 P, vec2 dPdx, vec2 dPdy) { return default; }

        // Do a texture lookup projectively and with explicit gradient as described in textureProjGrad, as well as with offset, as described in textureOffset.
        public static vec4 textureProjGradOffset(sampler2D sampler, vec3 P, vec2 dPdx, vec2 dPdy, ivec2 offset) { return default; }
        public static vec4 textureProjGradOffset(sampler2D sampler, vec4 P, vec2 dPdx, vec2 dPdy, ivec2 offset) { return default; }

        #endregion

        #region 8.9.4. Texture Gather Functions(1)

        // Returns the value: vec4(Sample_i0_j1(P, base).comp, Sample_i1_j1(P, base).comp, Sample_i1_j0(P, base).comp, Sample_i0_j0(P, base).comp)
        // If specified, the value of comp must be a constant integer expression with a value of 0, 1, 2, or 3, identifying the x, y, z, or w post-swizzled component of the four-component vector lookup result for each texel, respectively. If comp is not specified, it is treated as 0, selecting the x component of each texel to generate the result.
        public static vec4 textureGather(sampler2D sampler, vec2 P, int comp = default) { return default; }

        // Perform a texture gather operation as in textureGather by offset as described in textureOffset except that the offset can be variable (non constant) and the implementation- dependent minimum and maximum offset values are given by MIN_PROGRAM_TEXTURE_GATHER_OFFSET and MAX_PROGRAM_TEXTURE_GATHER_OFFSET, respectively.
        public static vec4 textureGatherOffset(sampler2D sampler, vec2 P, ivec2 offset, int comp = default) { return default; }

        // Operate identically to textureGatherOffset except that offsets is used to determine the location of the four texels to sample. Each of the four texels is obtained by applying the corresponding offset in offsets as a (u, v) coordinate offset to P, identifying the four-texel LINEAR footprint, and then selecting the texel i0 j0 of that footprint. The specified values in offsets must be constant integral expressions.
        public static vec4 textureGatherOffsets(sampler2D sampler, vec2 P, ivec2[] offsets, int comp = default) { return default; }

        #endregion

        #region 8.10. Atomic Counter Functions(1)

        // Atomically
        // 1. increments the counter for c, and
        // 2. returns its value prior to the increment operation.
        // These two steps are done atomically with respect to the atomic counter.
        public static uint atomicCounterIncrement(atomic_uint c) { return default; }

        // Atomically
        // 1. decrements the counter for c, and
        // 2. returns the value resulting from the decrement operation.
        // These two steps are done atomically with respect to the atomic counter.
        public static uint atomicCounterDecrement(atomic_uint c) { return default; }

        // Returns the counter value for c.
        public static uint atomicCounter(atomic_uint c) { return default; }

        // Atomically
        // 1. adds the value of data to the counter for c, and
        // 2. returns its value prior to the operation.
        // These two steps are done atomically with respect to the atomic counter.
        public static uint atomicCounterAdd(atomic_uint c, uint data) { return default; }

        // Atomically
        // 1. subtracts the value of data from the counter for c, and
        // 2. returns its value prior to the operation.
        // These two steps are done atomically with respect to the atomic counter.
        public static uint atomicCounterSubtract(atomic_uint c, uint data) { return default; }

        // Atomically
        // 1. sets the counter for c to the minimum of the value of the counter and the value of data, and
        // 2. returns the value prior to the operation.
        // These two steps are done atomically with respect to the atomic counter.
        public static uint atomicCounterMin(atomic_uint c, uint data) { return default; }

        // Atomically
        // 1. sets the counter for c to the maximum of the value of the counter and the value of data, and
        // 2. returns the value prior to the operation.
        // These two steps are done atomically with respect to the atomic counter.
        public static uint atomicCounterMax(atomic_uint c, uint data) { return default; }

        // Atomically
        // 1. sets the counter for c to the bitwise AND of the value of the counter and the value of data, and
        // 2. returns the value prior to the operation.
        // These two steps are done atomically with respect to the atomic counter.
        public static uint atomicCounterAnd(atomic_uint c, uint data) { return default; }

        // Atomically
        // 1. sets the counter for c to the bitwise OR of the value of the counter and the value of data, and
        // 2. returns the value prior to the operation.
        // These two steps are done atomically with respect to the atomic counter.
        public static uint atomicCounterOr(atomic_uint c, uint data) { return default; }

        // Atomically
        // 1. sets the counter for c to the bitwise XOR of the value of the counter and the value of data, and
        // 2. returns the value prior to the operation.
        // These two steps are done atomically with respect to the atomic counter.
        public static uint atomicCounterXor(atomic_uint c, uint data) { return default; }

        // Atomically
        // 1. sets the counter value for c to the value of data, and
        // 2. returns its value prior to the operation.
        // These two steps are done atomically with respect to the atomic counter.
        public static uint atomicCounterExchange(atomic_uint c, uint data) { return default; }

        // Atomically
        // 1. compares the value of compare and the counter value for c
        // 2. if the values are equal, sets the counter value for c to the value of data, and
        // 3. returns its value prior to the operation.
        // These three steps are done atomically with respect to the atomic counter.
        public static uint atomicCounterCompSwap(atomic_uint c, uint compare, uint data) { return default; }

        #endregion

        #region 8.11. Atomic Memory Functions(1)

        // Computes a new value by adding the value of data to the contents mem.
        public static uint atomicAdd(ref uint mem, uint data) { return default; }
        public static int atomicAdd(ref int mem, int data) { return default; }

        // Computes a new value by taking the minimum of the value of data and the contents of mem.
        public static uint atomicMin(ref uint mem, uint data) { return default; }
        public static int atomicMin(ref int mem, int data) { return default; }

        // Computes a new value by taking the maximum of the value of data and the contents of mem.
        public static uint atomicMax(ref uint mem, uint data) { return default; }
        public static int atomicMax(ref int mem, int data) { return default; }

        // Computes a new value by performing a bit-wise AND of the value of data and the contents of mem.
        public static uint atomicAnd(ref uint mem, uint data) { return default; }
        public static int atomicAnd(ref int mem, int data) { return default; }

        // Computes a new value by performing a bit-wise OR of the value of data and the contents of mem.
        public static uint atomicOr(ref uint mem, uint data) { return default; }
        public static int atomicOr(ref int mem, int data) { return default; }

        // Computes a new value by performing a bit-wise EXCLUSIVE OR of the value of data and the contents of mem.
        public static uint atomicXor(ref uint mem, uint data) { return default; }
        public static int atomicXor(ref int mem, int data) { return default; }

        // Computes a new value by simply copying the value of data.
        public static uint atomicExchange(ref uint mem, uint data) { return default; }
        public static int atomicExchange(ref int mem, int data) { return default; }

        // Compares the value of compare and the contents of mem. If the values are equal, the new value is given by data; otherwise, it is taken from the original contents of mem.
        public static uint atomicCompSwap(ref uint mem, uint compare, uint data) { return default; }
        public static int atomicCompSwap(ref int mem, int compare, int data) { return default; }

        #endregion

        #region 8.12. Image Functions (image2D only)(1)

        // Returns the dimensions of the image or images bound to image. For arrayed images, the last component of the return value will hold the size of the array. Cube images only return the dimensions of one face, and the number of cubes in the cube map array, if arrayed.
        // Note: The qualification readonly writeonly accepts a variable qualified with readonly, writeonly, both, or neither. It means the formal argument will be used for neither reading nor writing to the underlying memory.
        public static ivec2 imageSize(image2D image) { return default; }

        // Loads the texel at the coordinate P from the image unit image (in image2D image, ivec2 P). For multisample loads, the sample number is given by sample. When image, P, and sample identify a valid texel, the bits used to represent the selected texel in memory are converted to a vec4, ivec4, or uvec4 in the manner described in section 8.26 “Texture Image Loads and Stores” of the OpenGL Specification and returned.
        public static vec4 imageLoad(image2D image, ivec2 P) { return default; }

        // Stores data into the texel at the coordinate P from the image specified by image. For multisample stores, the sample number is given by sample. When image, P, and sample identify a valid texel, the bits used to represent data are converted to the format of the image unit in the manner described in section 8.26 “Texture Image Loads and Stores” of the OpenGL Specification and stored to the specified texel.
        public static void imageStore(image2D image, ivec2 P, vec4 data) { }

        // Computes a new value by adding the value of data to the contents of the selected texel.
        public static uint imageAtomicAdd(image2D image, ivec2 P, uint data) { return default; }
        public static int imageAtomicAdd(image2D image, ivec2 P, int data) { return default; }

        // Computes a new value by taking the minimum of the value of data and the contents of the selected texel.
        public static uint imageAtomicMin(image2D image, ivec2 P, uint data) { return default; }
        public static int imageAtomicMin(image2D image, ivec2 P, int data) { return default; }

        // Computes a new value by taking the maximum of the value data and the contents of the selected texel.
        public static uint imageAtomicMax(image2D image, ivec2 P, uint data) { return default; }
        public static int imageAtomicMax(image2D image, ivec2 P, int data) { return default; }

        // Computes a new value by performing a bit-wise AND of the value of data and the contents of the selected texel.
        public static uint imageAtomicAnd(image2D image, ivec2 P, uint data) { return default; }
        public static int imageAtomicAnd(image2D image, ivec2 P, int data) { return default; }

        // Computes a new value by performing a bit-wise OR of the value of data and the contents of the selected texel.
        public static uint imageAtomicOr(image2D image, ivec2 P, uint data) { return default; }
        public static int imageAtomicOr(image2D image, ivec2 P, int data) { return default; }

        // Computes a new value by performing a bit-wise EXCLUSIVE OR of the value of data and the contents of the selected texel.
        public static uint imageAtomicXor(image2D image, ivec2 P, uint data) { return default; }
        public static int imageAtomicXor(image2D image, ivec2 P, int data) { return default; }

        // Computes a new value by simply copying the value of data.
        public static uint imageAtomicExchange(image2D image, ivec2 P, uint data) { return default; }
        public static int imageAtomicExchange(image2D image, ivec2 P, int data) { return default; }
        public static Float imageAtomicExchange(image2D image, ivec2 P, Float data) { return default; }

        // Compares the value of compare and the contents of the selected texel. If the values are equal, the new value is given by data; otherwise, it is taken from the original value loaded from the texel.
        public static uint imageAtomicCompSwap(image2D image, ivec2 P, uint compare, uint data) { return default; }
        public static int imageAtomicCompSwap(image2D image, ivec2 P, int compare, int data) { return default; }

        #endregion

        #region 8.14.2. Interpolation Functions

        // Returns the value of the input interpolant sampled at a location inside both the pixel and the primitive being processed. The value obtained would be the same value assigned to the input variable if declared with the centroid qualifier.
        public static Float interpolateAtCentroid(Float interpolant) { return default; }
        public static vec2 interpolateAtCentroid(vec2 interpolant) { return default; }
        public static vec3 interpolateAtCentroid(vec3 interpolant) { return default; }
        public static vec4 interpolateAtCentroid(vec4 interpolant) { return default; }

        // Returns the value of the input interpolant variable at the location of sample number sample. If multisample buffers are not available, the input variable will be evaluated at the center of the pixel. If sample sample does not exist, the position used to interpolate the input variable is undefined.
        public static Float interpolateAtSample(Float interpolant, int sample) { return default; }
        public static vec2 interpolateAtSample(vec2 interpolant, int sample) { return default; }
        public static vec3 interpolateAtSample(vec3 interpolant, int sample) { return default; }
        public static vec4 interpolateAtSample(vec4 interpolant, int sample) { return default; }

        // Returns the value of the input interpolant variable sampled at an offset from the center of the pixel specified by offset. The two floating-point components of offset, give the offset in pixels in the x and y directions, respectively. An offset of (0, 0) identifies the center of the pixel. The range and granularity of offsets supported by this function is implementation-dependent.
        public static Float interpolateAtOffset(Float interpolant, vec2 offset) { return default; }
        public static vec2 interpolateAtOffset(vec2 interpolant, vec2 offset) { return default; }
        public static vec3 interpolateAtOffset(vec3 interpolant, vec2 offset) { return default; }
        public static vec4 interpolateAtOffset(vec4 interpolant, vec2 offset) { return default; }

        #endregion

        #region 8.16. Shader Invocation Control Functions

        // For any given static instance of barrier(), all tessellation control shader invocations for a single input patch must enter it before any will be allowed to continue beyond it, or all compute shader invocations for a single workgroup must enter it before any will continue beyond it.
        public static void barrier() { }

        #endregion

        #region 8.17. Shader Memory Control Functions

        // Control the ordering of memory transactions issued by a single shader invocation.
        public static void memoryBarrier() { }

        // Control the ordering of accesses to atomic-counter variables issued by a single shader invocation.
        public static void memoryBarrierAtomicCounter() { }

        // Control the ordering of memory transactions to buffer variables issued within a single shader invocation.
        public static void memoryBarrierBuffer() { }

        // Control the ordering of memory transactions to shared variables issued within a single shader invocation, as viewed by other invocations in the same workgroup. Only available in compute shaders.
        public static void memoryBarrierShared() { }

        // Control the ordering of memory transactions to images issued within a single shader invocation.
        public static void memoryBarrierImage() { }

        // Control the ordering of all memory transactions issued within a single shader invocation, as viewed by other invocations in the same workgroup. Only available in compute shaders.
        public static void groupMemoryBarrier() { }

        #endregion

        #region 8.19. Shader Invocation Group Functions

        // Returns true if and only if value is true for at least one active invocation in the group.
        public static bool anyInvocation(bool value) { return default; }

        // Returns true if and only if value is true for all active invocations in the group.
        public static bool allInvocations(bool value) { return default; }

        // Returns true if value is the same for all active invocations in the group.
        public static bool allInvocationsEqual(bool value) { return default; }

        #endregion
    }

    // genFType: float, vec2, vec3, vec4
    // genIType: int, ivec2, ivec3, ivec4
    // genUType: uint, uvec2, uvec3, uvec4
    // genBType: bool, bvec2, bvec3, bvec4
    // genDType: double, dvec2, dvec3, dvec4
    public static partial class EnvironmentFunctions<genFType, genIType, genUType, genBType, genDType>
    {
        #region 8.1. Angle and Trigonometry Functions

        // Converts degrees to radians, i.e., (π / 180) · degrees.
        public static genFType radians(genFType degrees) { return default; }

        // Converts radians to degrees, i.e., (180 / π) · radians.
        public static genFType degrees(genFType radians) { return default; }

        // The standard trigonometric sine function.
        public static genFType sin(genFType angle) { return default; }

        // The standard trigonometric cosine function.
        public static genFType cos(genFType angle) { return default; }

        // The standard trigonometric tangent.
        public static genFType tan(genFType angle) { return default; }

        // Arc sine. Returns an angle whose sine is x. The range of values returned by this function is [-π / 2, π / 2]. Results are undefined if |x| > 1.
        public static genFType asin(genFType x) { return default; }

        // Arc cosine. Returns an angle whose cosine is x. The range of values returned by this function is [0,π]. Results are undefined if |x| > 1.
        public static genFType acos(genFType x) { return default; }

        // Arc tangent. Returns an angle whose tangent is y / x. The signs of x and y are used to determine what quadrant the angle is in. The range of values returned by this function is [-π, π. Results are undefined if x and y are both 0.
        public static genFType atan(genFType y, genFType x) { return default; }

        // Arc tangent. Returns an angle whose tangent is y_over_x. The range of values returned by this function is [-π / 2, π / 2].
        public static genFType atan(genFType y_over_x) { return default; }

        // Returns the hyperbolic sine function (ex - e-x) / 2.
        public static genFType sinh(genFType x) { return default; }

        // Returns the hyperbolic cosine function (ex + e-x) / 2.
        public static genFType cosh(genFType x) { return default; }

        // Returns the hyperbolic tangent function sinh(x) / cosh(x).
        public static genFType tanh(genFType x) { return default; }

        // Arc hyperbolic sine; returns the inverse of sinh.
        public static genFType asinh(genFType x) { return default; }

        // Arc hyperbolic cosine; returns the non-negative inverse of cosh. Results are undefined if x < 1.
        public static genFType acosh(genFType x) { return default; }

        // Arc hyperbolic tangent; returns the inverse of tanh. Results are undefined if x ≥ 1.
        public static genFType atanh(genFType x) { return default; }

        #endregion

        #region 8.2. Exponential Functions

        // Returns x raised to the y power, i.e., x^y. Results are undefined if x < 0. Results are undefined if x = 0 and y ≤ 0.
        public static genFType pow(genFType x, genFType y) { return default; }

        // Returns the natural exponentiation of x, i.e., e^x.
        public static genFType exp(genFType x) { return default; }

        // Returns the natural logarithm of x, i.e., returns the value y which satisfies the equation x = e^y. Results are undefined if x ≤ 0.
        public static genFType log(genFType x) { return default; }

        // Returns 2 raised to the x power, i.e., 2^x.
        public static genFType exp2(genFType x) { return default; }

        // Returns the base 2 logarithm of x, i.e., returns the value y which satisfies the equation x = 2^y. Results are undefined if x ≤ 0.
        public static genFType log2(genFType x) { return default; }

        // Returns sqrt(x). Results are undefined if x < 0.
        public static genFType sqrt(genFType x) { return default; }
        public static genDType sqrt(genDType x) { return default; }

        // Returns 1 / sqrt(x). Results are undefined if x ≤ 0.
        public static genFType inversesqrt(genFType x) { return default; }
        public static genDType inversesqrt(genDType x) { return default; }

        #endregion

        #region 8.3. Common Functions

        // Returns x if x ≥ 0; otherwise it returns -x.
        public static genFType abs(genFType x) { return default; }
        public static genIType abs(genIType x) { return default; }
        public static genDType abs(genDType x) { return default; }

        // Returns 1.0 if x > 0, 0.0 if x = 0, or -1.0 if x < 0.
        public static genFType sign(genFType x) { return default; }
        public static genIType sign(genIType x) { return default; }
        public static genDType sign(genDType x) { return default; }

        // Returns a value equal to the nearest integer that is less than or equal to x.
        public static genFType floor(genFType x) { return default; }
        public static genDType floor(genDType x) { return default; }

        // Returns a value equal to the nearest integer to x whose absolute value is not larger than the absolute value of x.
        public static genFType trunc(genFType x) { return default; }
        public static genDType trunc(genDType x) { return default; }

        // Returns a value equal to the nearest integer to x. The fraction 0.5 will round in a direction chosen by the implementation, presumably the direction that is fastest. This includes the possibility that round(x) returns the same value as roundEven(x) for all values of x.
        public static genFType round(genFType x) { return default; }
        public static genDType round(genDType x) { return default; }

        // Returns a value equal to the nearest integer to x. A fractional part of 0.5 will round toward the nearest even integer. (Both 3.5 and 4.5 for x will return 4.0.)
        public static genFType roundEven(genFType x) { return default; }
        public static genDType roundEven(genDType x) { return default; }

        // Returns a value equal to the nearest integer that is greater than or equal to x.
        public static genFType ceil(genFType x) { return default; }
        public static genDType ceil(genDType x) { return default; }

        // Returns x - floor(x).
        public static genFType fract(genFType x) { return default; }
        public static genDType fract(genDType x) { return default; }

        // Modulus. Returns x - y · floor(x / y).
        // Note that implementations may use a cheap approximation to the remainder, and the error can be large due to the discontinuity in floor. This can produce mathematically unexpected results in some cases, such as mod(x,x) computing x rather than 0, and can also cause the result to have a different sign than the infinitely precise result.
        public static genFType mod(genFType x, Float y) { return default; }
        public static genFType mod(genFType x, genFType y) { return default; }
        public static genDType mod(genDType x, double y) { return default; }
        public static genDType mod(genDType x, genDType y) { return default; }

        // Returns the fractional part of x and sets i to the integer part (as a whole number floating-point value). Both the return value and the output parameter will have the same sign as x.
        public static genFType modf(genFType x, out genFType i) { i = default; return default; }
        public static genDType modf(genDType x, out genDType i) { i = default; return default; }

        // Returns y if y < x; otherwise it returns x.
        public static genFType min(genFType x, genFType y) { return default; }
        public static genFType min(genFType x, Float y) { return default; }
        public static genDType min(genDType x, genDType y) { return default; }
        public static genDType min(genDType x, double y) { return default; }
        public static genIType min(genIType x, genIType y) { return default; }
        public static genIType min(genIType x, int y) { return default; }
        public static genUType min(genUType x, genUType y) { return default; }
        public static genUType min(genUType x, uint y) { return default; }

        // Returns y if x < y; otherwise it returns x.
        public static genFType max(genFType x, genFType y) { return default; }
        public static genFType max(genFType x, Float y) { return default; }
        public static genDType max(genDType x, genDType y) { return default; }
        public static genDType max(genDType x, double y) { return default; }
        public static genIType max(genIType x, genIType y) { return default; }
        public static genIType max(genIType x, int y) { return default; }
        public static genUType max(genUType x, genUType y) { return default; }
        public static genUType max(genUType x, uint y) { return default; }

        // Returns min(max(x, minVal), maxVal). Results are undefined if minVal > maxVal.
        public static genFType clamp(genFType x, genFType minVal, genFType maxVal) { return default; }
        public static genFType clamp(genFType x, Float minVal, Float maxVal) { return default; }
        public static genDType clamp(genDType x, genDType minVal, genDType maxVal) { return default; }
        public static genDType clamp(genDType x, double minVal, double maxVal) { return default; }
        public static genIType clamp(genIType x, genIType minVal, genIType maxVal) { return default; }
        public static genIType clamp(genIType x, int minVal, int maxVal) { return default; }
        public static genUType clamp(genUType x, genUType minVal, genUType maxVal) { return default; }
        public static genUType clamp(genUType x, uint minVal, uint maxVal) { return default; }

        // Returns the linear blend of x and y, i.e., x · (1 - a) + y · a.
        public static genFType mix(genFType x, genFType y, genFType a) { return default; }
        public static genFType mix(genFType x, genFType y, Float a) { return default; }
        public static genDType mix(genDType x, genDType y, genDType a) { return default; }
        public static genDType mix(genDType x, genDType y, double a) { return default; }

        // Selects which vector each returned component comes from. For a component of a that is false, the corresponding component of x is returned.
        // For a component of a that is true, the corresponding component of y is returned.
        // Components of x and y that are not selected are allowed to be invalid floating-point values and will have no effect on the results. Thus, this provides different functionality than, for example,
        // genFType mix(genFType x, genFType y, genFType(a)) where a is a Boolean vector.
        public static genFType mix(genFType x, genFType y, genBType a) { return default; }
        public static genDType mix(genDType x, genDType y, genBType a) { return default; }
        public static genIType mix(genIType x, genIType y, genBType a) { return default; }
        public static genUType mix(genUType x, genUType y, genBType a) { return default; }
        public static genBType mix(genBType x, genBType y, genBType a) { return default; }

        // Returns 0.0 if x < edge; otherwise it returns 1.0.
        public static genFType step(genFType edge, genFType x) { return default; }
        public static genFType step(Float edge, genFType x) { return default; }
        public static genDType step(genDType edge, genDType x) { return default; }
        public static genDType step(double edge, genDType x) { return default; }

        // Returns 0.0 if x ≤ edge0 and 1.0 if x ≥ edge1, and performs smooth Hermite interpolation between 0 and 1 when edge0 < x < edge1. This is useful in cases where you would want a threshold function with a smooth transition.
        // This is equivalent to:
        //     genFType t;
        //     t = clamp ((x - edge0) / (edge1 -
        //     edge0), 0, 1);
        //     return t * t * (3 - 2 * t);
        // (And similarly for doubles.) Results are undefined if edge0 ≥ edge1.
        public static genFType smoothstep(genFType edge0, genFType edge1, genFType x) { return default; }
        public static genFType smoothstep(Float edge0, Float edge1, genFType x) { return default; }
        public static genDType smoothstep(genDType edge0, genDType edge1, genDType x) { return default; }
        public static genDType smoothstep(double edge0, double edge1, genDType x) { return default; }

        // Returns true if x holds a NaN. Returns false otherwise. Always returns false if NaNs are not implemented.
        public static genBType isnan(genFType x) { return default; }
        public static genBType isnan(genDType x) { return default; }

        // Returns true if x holds a positive infinity or negative infinity. Returns false otherwise.
        public static genBType isinf(genFType x) { return default; }
        public static genBType isinf(genDType x) { return default; }

        // Returns a signed or unsigned integer value representing the encoding of a floating-point value. The Float value’s bit-level representation is preserved.
        public static genIType floatBitsToInt(genFType value) { return default; }
        public static genUType floatBitsToUint(genFType value) { return default; }

        // Returns a floating-point value corresponding to a signed or unsigned integer encoding of a floating-point value. If a NaN is passed in, it will not signal, and the resulting value is unspecified. If an Inf is passed in, the resulting value is the corresponding Inf. If a subnormal number is passed in, the result might be flushed to 0. Otherwise, the bit-level representation is preserved.
        public static genFType intBitsToFloat(genIType value) { return default; }
        public static genFType uintBitsToFloat(genUType value) { return default; }

        // Computes and returns a * b + c. In uses where the return value is eventually consumed by a variable declared as precise:
        // • fma() is considered a single operation, whereas the expression a * b + c consumed by a variable declared precise is considered two operations.
        // • The precision of fma() can differ from the precision of the expression a * b + c.
        // • fma() will be computed with the same precision as any other fma() consumed by a precise variable, giving invariant results for the same input values of a, b, and c. Otherwise, in the absence of precise consumption, there are no special constraints on the number of operations or difference in precision between fma() and the expression a * b + c.
        public static genFType fma(genFType a, genFType b, genFType c) { return default; }
        public static genDType fma(genDType a, genDType b, genDType c) { return default; }

        // Splits x into a floating-point significand in the range [0.5,1.0], and an integral exponent of two, such that
        // x = significant · 2^exponent
        // The significand is returned by the function and the exponent is returned in the parameter exp. For a floating-point value of zero, the significand and exponent are both zero.
        // If an implementation supports signed zero, an input value of minus zero should return a significand of minus zero. For a floating-point value that is an infinity or is not a number, the results are undefined.
        // If the input x is a vector, this operation is performed in a component-wise manner; the value returned by the function and the value written to exp are vectors with the same number of components as x.
        public static genFType frexp(genFType x, out genIType exp) { exp = default; return default; }

        // Splits x into a floating-point significand in the range [0.5,1.0], and an integral exponent of two, such that x = significant · 2*exponent
        public static genDType frexp(genDType x, out genIType exp) { exp = default; return default; }

        // Builds a floating-point number from x and the corresponding integral exponent of two in exp, returning:
        // significand · 2^exponent
        // If this product is too large to be represented in the floating-point type, the result is undefined.
        // If exp is greater than +128 (single-precision) or +1024 (double-precision), the value returned is undefined. If exp is less than -126 (single- precision) or -1022 (double-precision), the value returned may be flushed to zero. Additionally, splitting the value into a significand and exponent using frexp() and then reconstructing a floating-point value using ldexp() should yield the original input for zero and all finite non- subnormal values.
        // If the input x is a vector, this operation is performed in a component-wise manner; the value passed in exp and returned by the function are vectors with the same number of components as x.
        public static genFType ldexp(genFType x, genIType exp) { return default; }
        public static genDType ldexp(genDType x, genIType exp) { return default; }

        #endregion

        #region 8.5. Geometric Functions

        // Returns the length of vector x, i.e., sqrt( x0^2 + x1^2 + … ).
        public static Float length(genFType x) { return default; }
        public static double length(genDType x) { return default; }

        // Returns the distance between p0 and p1, i.e., length(p0 - p1)
        public static Float distance(genFType p0, genFType p1) { return default; }
        public static double distance(genDType p0, genDType p1) { return default; }

        // Returns the dot product of x and y, i.e., x0 · y0 + x1 · y1 + …
        public static Float dot(genFType x, genFType y) { return default; }
        public static double dot(genDType x, genDType y) { return default; }

        // Returns a vector in the same direction as x but with a length of 1, i.e. x / length(x). compatibility profile only
        public static genFType normalize(genFType x) { return default; }
        public static genDType normalize(genDType x) { return default; }

        // If dot(Nref, I) < 0 return N, otherwise return -N.
        public static genFType faceforward(genFType N, genFType I, genFType Nref) { return default; }
        public static genDType faceforward(genDType N, genDType I, genDType Nref) { return default; }

        // For the incident vector I and surface orientation N, returns the reflection direction: I - 2 · dot(N, I) · N. N must already be normalized in order to achieve the desired result.
        public static genFType reflect(genFType I, genFType N) { return default; }
        public static genDType reflect(genDType I, genDType N) { return default; }

        // For the incident vector I and surface normal N, and the ratio of indices of refraction eta, return the refraction vector. The result is computed by the refraction equation shown below.
        // The input parameters for the incident vector _I_ and the surface normal _N_ must already be normalized to get the desired results.
        public static genFType refract(genFType I, genFType N, Float eta) { return default; }
        public static genDType refract(genDType I, genDType N, double eta) { return default; }

        #endregion

        #region 8.8. Integer Functions

        // Adds 32-bit unsigned integers x and y, returning the sum modulo 232. The value carry is set to zero if the sum was less than 232, or one otherwise.
        public static genUType uaddCarry(genUType x, genUType y, out genUType carry) { carry = default; return default; }

        // Subtracts the 32-bit unsigned integer y from x, returning the difference if non-negative, or 232 plus the difference otherwise. The value borrow is set to zero if x ≥ y, or one otherwise.
        public static genUType usubBorrow(genUType x, genUType y, out genUType borrow) { borrow = default; return default; }

        // Multiplies 32-bit unsigned or signed integers x and y, producing a 64-bit result. The 32 least-significant bits are returned in lsb. The 32 most-significant bits are returned in msb.
        public static void umulExtended(genUType x, genUType y, out genUType msb, out genUType lsb) { msb = default; lsb = default; }
        public static void imulExtended(genIType x, genIType y, out genIType msb, out genIType lsb) { msb = default; lsb = default; }

        // Extracts bits [offset, offset + bits - 1] from value, returning them in the least significant bits of the result.
        // For unsigned data types, the most significant bits of the result will be set to zero. For signed data types, the most significant bits will be set to the value of bit offset + bits - 1.
        // If bits is zero, the result will be zero. The result will be undefined if offset or bits is negative, or if the sum of offset and bits is greater than the number of bits used to store the operand. Note that for vector versions of bitfieldExtract(), a single pair of offset and bits values is shared for all components.
        public static genIType bitfieldExtract(genIType value, int offset, int bits) { return default; }
        public static genUType bitfieldExtract(genUType value, int offset, int bits) { return default; }

        // Inserts the bits least significant bits of insert into base.
        // The result will have bits [offset, offset + bits - 1] taken from bits [0, bits - 1] of insert, and all other bits taken directly from the corresponding bits of base. If bits is zero, the result will simply be base. The result will be undefined if offset or bits is negative, or if the sum of offset and bits is greater than the number of bits used to store the operand.
        // Note that for vector versions of bitfieldInsert(), a single pair of offset and bits values is shared for all components.
        public static genIType bitfieldInsert(genIType _base, genIType insert, int offset, int bits) { return default; }
        public static genUType bitfieldInsert(genUType _base, genUType insert, int offset, int bits) { return default; }

        // Reverses the bits of value. The bit numbered n of the result will be taken from bit (bits - 1) - n of value, where bits is the total number of bits used to represent value.
        public static genIType bitfieldReverse(genIType value) { return default; }
        public static genUType bitfieldReverse(genUType value) { return default; }

        // Returns the number of one bits in the binary representation of value.
        public static genIType bitCount(genIType value) { return default; }
        public static genIType bitCount(genUType value) { return default; }

        // Returns the bit number of the least significant one bit in the binary representation of value. If value is zero, -1 will be returned.
        public static genIType findLSB(genIType value) { return default; }
        public static genIType findLSB(genUType value) { return default; }

        // Returns the bit number of the most significant bit in the binary representation of value. For positive integers, the result will be the bit number of the most significant one bit. For negative integers, the result will be the bit number of the most significant zero bit. For a value of zero or negative one, -1 will be returned.
        public static genIType findMSB(genIType value) { return default; }
        public static genIType findMSB(genUType value) { return default; }

        #endregion

        #region 8.14.1. Derivative Functions

        // Returns either dFdxFine(p) or dFdxCoarse(p), based on implementation choice, presumably whichever is the faster, or by whichever is selected in the API through quality-versus-speed hints.
        public static genFType dFdx(genFType p) { return default; }

        // Returns either dFdyFine(p) or dFdyCoarse(p), based on implementation choice, presumably whichever is the faster, or by whichever is selected in the API through quality-versus-speed hints.
        public static genFType dFdy(genFType p) { return default; }

        // Returns the partial derivative of p with respect to the window x coordinate. Will use local differencing based on the value of p for the current fragment and its immediate neighbor(s).
        public static genFType dFdxFine(genFType p) { return default; }

        // Returns the partial derivative of p with respect to the window y coordinate. Will use local differencing based on the value of p for the current fragment and its immediate neighbor(s).
        public static genFType dFdyFine(genFType p) { return default; }

        // Returns the partial derivative of p with respect to the window x coordinate. Will use local differencing based on the value of p for the current fragment’s neighbors, and will possibly, but not necessarily, include the value of p for the current fragment. That is, over a given area, the implementation can x compute derivatives in fewer unique locations than would be allowed for dFdxFine(p)
        public static genFType dFdxCoarse(genFType p) { return default; }

        // Returns the partial derivative of p with respect to the window y coordinate. Will use local differencing based on the value of p for the current fragment’s neighbors, and will possibly, but not necessarily, include the value of p for the current fragment. That is, over a given area, the implementation can compute y derivatives in fewer unique locations than would be allowed for dFdyFine(p).
        public static genFType dFdyCoarse(genFType p) { return default; }

        // Returns abs(dFdx(p)) + abs(dFdy(p)).
        public static genFType fwidth(genFType p) { return default; }

        // Returns abs(dFdxFine(p)) + abs(dFdyFine(p)).
        public static genFType fwidthFine(genFType p) { return default; }

        // Returns abs(dFdxCoarse(p)) + abs(dFdyCoarse(p))
        public static genFType fwidthCoarse(genFType p) { return default; }

        #endregion
    }

    // vec: vec2, vec3, vec4
    // ivec: ivec2, ivec3, ivec4
    // uvec: uvec2, uvec3, uvec4
    // bvec: bvec2, bvec3, bvec4
    // dvec: dvec2, dvec3, dvec4
    public static partial class EnvironmentVectorFunctions<vec, ivec, uvec, bvec, dvec>
    {
        #region 8.7. Vector Relational Functions

        // Returns the component-wise compare of x < y.
        public static bvec lessThan(vec x, vec y) { return default; }
        public static bvec lessThan(ivec x, ivec y) { return default; }
        public static bvec lessThan(uvec x, uvec y) { return default; }
        public static bvec lessThan(dvec x, dvec y) { return default; }

        // Returns the component-wise compare of x ≤ y.
        public static bvec lessThanEqual(vec x, vec y) { return default; }
        public static bvec lessThanEqual(ivec x, ivec y) { return default; }
        public static bvec lessThanEqual(uvec x, uvec y) { return default; }
        public static bvec lessThanEqual(dvec x, dvec y) { return default; }

        // Returns the component-wise compare of x > y.
        public static bvec greaterThan(vec x, vec y) { return default; }
        public static bvec greaterThan(ivec x, ivec y) { return default; }
        public static bvec greaterThan(uvec x, uvec y) { return default; }
        public static bvec greaterThan(dvec x, dvec y) { return default; }

        // Returns the component-wise compare of x ≥ y.
        public static bvec greaterThanEqual(vec x, vec y) { return default; }
        public static bvec greaterThanEqual(ivec x, ivec y) { return default; }
        public static bvec greaterThanEqual(uvec x, uvec y) { return default; }
        public static bvec greaterThanEqual(dvec x, dvec y) { return default; }

        // Returns the component-wise compare of x == y.
        public static bvec equal(vec x, vec y) { return default; }
        public static bvec equal(ivec x, ivec y) { return default; }
        public static bvec equal(uvec x, uvec y) { return default; }
        public static bvec equal(dvec x, dvec y) { return default; }
        public static bvec equal(bvec x, bvec y) { return default; }

        // Returns the component-wise compare of x ≠ y.
        public static bvec notEqual(vec x, vec y) { return default; }
        public static bvec notEqual(ivec x, ivec y) { return default; }
        public static bvec notEqual(uvec x, uvec y) { return default; }
        public static bvec notEqual(dvec x, dvec y) { return default; }
        public static bvec notEqual(bvec x, bvec y) { return default; }

        // Returns true if any component of x is true.
        public static bool any(bvec x) { return default; }

        // Returns true only if all components of x are true.
        public static bool all(bvec x) { return default; }

        // Returns the component-wise logical complement of x.
        public static bvec not(bvec x) { return default; }

        #endregion
    }
}