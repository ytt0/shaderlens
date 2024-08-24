Workarounds
===========
While it's not required to have the C# code without any syntax errors in order to edit it in Visual Studio, or convert it to valid GLSL code, it's still very convenient to use code analysis where C# syntax errors could indicate possible GLSL errors as well. |br|
To keep the errors list in Visual Studio clean, and still have the C# code convertible to a valid GLSL code, the following workarounds could be applied:


.. _csharp-workaround-floating-point:

Floating Points
---------------

C# literals are of type :csharp:`double` by default (without an ``f`` suffix), while GLSL literals are of type :csharp:`float`.
This is causing casting errors (`CS0664 <https://learn.microsoft.com/en-us/dotnet/csharp/misc/cs0664>`_) while writing a "GLSL like" code in C#, were :csharp:`double` literals are assigned to :csharp:`float` value types.

The :doc:`environment` provides a :csharp:`Float` type, with :csharp:`_float`, and :csharp:`float_` aliases, that has an implicit casting between :csharp:`float` and :csharp:`double`, and can be used to eliminated these errors in most cases.

.. note::

    When a casting error is encountered, replacing a :csharp:`float` type with a :csharp:`_float` or :csharp:`float_`, could eliminate it, in other cases an ``f`` suffix could be added to the literal instead.

.. code-block:: glsl
    :caption: Intended GLSL code

    float x = 1.23;

.. code-block:: csharp
    :caption: C# workarounds

    // using a double literal
    _float x = 1.23; // Float type alias
    float_ x = 1.23; // Float type alias

    // using a float literal (the suffix would be removed)
    float x = 1.23f; // float primitive type

For methods declaration (input parameters, and return type), it's recommended to always use the :csharp:`Float` type instead of the :csharp:`float` primitive, so that :csharp:`double` literals could be passed as parameters values, or returned without casting.

Example:

    .. code-block:: csharp
        :emphasize-lines: 3,9,19

        // use a "Float" type (float_), instead of a float primitive, for both
        // input parameter type, and return type.
        float_ sqrtSafe(float_ value)
        {
            if (value < 0.0)
            {
                // a double literal can be used as a return value (no need for
                // a suffix, "0.0f")
                return 0.0;
            }

            return sqrt(value);
        }

        ...

        // a double literal can be used as an input parameter value (no need for
        // a suffix, "2.0f")
        float result = sqrtSafe(2.0);

Casts
-----
C# cast syntax is different from GLSL, for example a cast from :csharp:`float` to :csharp:`int` is :csharp:`(int)1.0` in C# and :glsl:`int(1.0)` in GLSL.
The :csharp:`_int()` or :csharp:`int_()` :doc:`environment` methods could be used instead.

Similar methods are also defined for :glsl:`bool`, :glsl:`uint`, and :glsl:`float`.

.. code-block:: glsl
    :caption: Intended GLSL code

    int x = int(position.x);

.. code-block:: csharp
    :caption: C# workarounds

    int x = _int(position.x);
    int x = int_(position.x);

.. _csharp-workaround-consts:

Uniforms
--------
To declare a uniform, a field could be used, with a :ref:`uniform<conversion-rules-uniform>` conversion rule, a :csharp:`const` or :csharp:`readonly` modifier could be added to reduce warnings, and would be removed by the rule. |br|
The conversion rule can have additional line annotation properties that would be preserved, and parsed as part of the GLSL code.

.. code-block:: glsl
    :caption: Intended GLSL code

    //@uniform, min: 0.0, step: 0.1
    uniform vec2 value = vec2(1.0, 2.0);

.. code-block:: csharp
    :caption: C# workaround

    //@uniform, min: 0.0, step: 0.1
    readonly vec2 value = vec2(1.0, 2.0);

Consts
------
:doc:`environment` types such as :glsl:`vec2`, :glsl:`mat3` are not primitive types, and cannot be declared with the :csharp:`const` modifier, instead a :csharp:`readonly` modifier, or a :ref:`const conversion rule<conversion-rules-const>` (for local variables) could be used.

.. code-block:: glsl
    :caption: Intended GLSL code

    const vec2 value = vec2(1.0, 2.0);

.. code-block:: csharp
    :caption: C# workarounds
    :emphasize-lines: 3,7

    class Image
    {
        readonly vec2 value = vec2(1.0, 2.0);

        float getValue()
        {
            //@const
            vec2 value = vec2(1.0, 2.0);
        }
    }

Defines
-------
C# does not support define directives with a value, or macros, a :ref:`define<conversion-rules-define>` conversion rule could to be used instead.

.. code-block:: glsl
    :caption: Intended GLSL code:

    #define PI 3.14159265

.. code-block:: csharp
    :caption: C# workaround

    //@define
    float PI = 3.14159265f;

.. code-block:: glsl
    :caption: Intended GLSL code

    #define sqr(x) ((x) * (x))

.. code-block:: csharp
    :caption: C# workaround

    //@define
    float sqr(float x) => ((x) * (x));

Overloads could be added with a :ref:`remove-line<conversion-rules-remove-line>` conversion rule.

.. code-block:: csharp

    //@remove-line
    vec2 sqr(vec2 x) => x * x;

    //@remove-line
    vec3 sqr(vec3 x) => x * x;


Automatic Conversion
--------------------

.. _csharp-workaround-arrays:

Arrays
    C# array initialization uses curly brackets, while GLSL uses round ones. |br|
    Arrays can be automatically converted when a type is specified (use :csharp:`new int[]`, and not :csharp:`new[]`).

    - :csharp:`new int[]{ 1, 2, 3 }` (C#) is converted to :glsl:`int[]( 1, 2, 3 )` (GLSL).

    A multi-line array conversion is also supported.

.. _csharp-workaround-directives:

Preprocessor Directives
    C# uses the :csharp:`#if` directive for both conditions and symbols testing, while GLSL uses :glsl:`#if` for conditions, and  :glsl:`#ifdef` / :glsl:`#ifndef` for symbols.

    Symbol testing directives are automatically converted.

    - :csharp:`#if A` (C#) is converted to :glsl:`#ifdef A` (GLSL)
    - :csharp:`#if !A` (C#) is converted to :glsl:`#ifndef A` (GLSL)

    More advanced directives could be converted with custom :ref:`line<conversion-rules-line>` rules.

.. _csharp-workaround-modifiers:

Modifiers
    C# modifiers are automatically removed or replaced.

    - :csharp:`ref` and :csharp:`out` parameter modifiers are removed from functions calls, and :csharp:`ref` is converted to :glsl:`inout` in functions declarations.

        - Declaration conversion: |br|
          :csharp:`void modifyValue(ref int value) { ... }` (C#) |br|
          :glsl:`void modifyValue(inout int value) { ... }` (GLSL) |br|
          |br|
        - Call conversion: |br|
          :csharp:`modifyValue(ref value);` (C#) |br|
          :glsl:`modifyValue(value);` (GLSL) |br| |br|
          :csharp:`getValue(out value);` (C#) |br|
          :glsl:`getValue(value);` (GLSL)

    -
        :csharp:`public`
        :csharp:`private`
        :csharp:`internal`
        :csharp:`protected`
        :csharp:`virtual`
        :csharp:`override`
        :csharp:`static`
        modifiers are removed.
    - :csharp:`readonly` modifier is converted to :glsl:`const`.

Other Workarounds
-----------------
Other incompatibilities between C# and GLSL should be resolvable by using :ref:`replace-line<conversion-rules-replace-line>`, or other :doc:`custom conversion rules</manual/csharp/conversion-rules>`.
