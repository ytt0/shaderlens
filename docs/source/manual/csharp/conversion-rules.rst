Conversion Rules
================

Conversion rules allow converting a valid C# code, into a valid GLSL code, so that the code could be edited as C# in Visual Studio, and still compile as GLSL by the viewer.

Conversion rules are defined using :doc:`line annotations</manual/line-annotation>`.

.. _conversion-rules-built-in:

Built-in Rules
--------------

A predefined set of conversion rules, that are used as a starting point and are covering most of the common cases.
Additional rules could be added for more specific cases, and would have higher priority over the built-in rules.

- :csharp:`namespace` and :csharp:`using` declaration lines are automatically removed.
- :csharp:`namespace` and :csharp:`class` indentations are automatically reduced.
- :csharp:`#region` and :csharp:`#endregion` lines are automatically removed.
- C# array initialization is automatically converted to GLSL array initialization (see :ref:`arrays workarounds<csharp-workaround-arrays>`).
- C# keywords are removed
  :csharp:`public`
  :csharp:`private`
  :csharp:`internal`
  :csharp:`protected`
  :csharp:`virtual`
  :csharp:`override`
  :csharp:`static`
  :csharp:`new`
  (see :ref:`modifiers workarounds<csharp-workaround-modifiers>`).
- C# explicit castings are removed :csharp:`(float)`, :csharp:`(double)` (see :ref:`floating points workarounds<csharp-workaround-floating-point>`).
- Underscores are removed from :doc:`environment`'s casting methods and type aliases for
  :csharp:`bool`,
  :csharp:`int`,
  :csharp:`uint`, and
  :csharp:`float`.
- :doc:`environment` :csharp:`Float` is replaced with :glsl:`float`.
- Floating point ``f``/``F`` suffix is removed.
- :csharp:`readonly` is automatically converted to :glsl:`const` (see :ref:`const workarounds<csharp-workaround-consts>`).
- :csharp:`ref` is automatically converted to :glsl:`inout` in functions declaration.
- :csharp:`ref` and :csharp:`out` parameter modifiers in functions calls are removed (see :ref:`modifiers workarounds<csharp-workaround-modifiers>`).
- :csharp:`#if A` and :csharp:`#if !A` are replaced with :glsl:`#ifdef A` and :glsl:`#ifndef A` (see :ref:`preprocessor directives workarounds<csharp-workaround-directives>`).

.. _conversion-rules-line:

Line Rules
----------

Conversion rules that convert an entire line.

.. _conversion-rules-replace-line:

Replace Line
^^^^^^^^^^^^
    Replace a C# compatible line, with a GLSL compatible line.

    Rule:
        :csharp:`//@replace-line: <value>`

        :value: A line end terminated value.

    Example:
        .. code-block:: csharp

            //@replace-line: float value = float(2); // GLSL casting
            float value = (float)2; // C# casting

Insert Line
^^^^^^^^^^^
    Insert a line that is only needed in GLSL.

    Rule:
        :csharp:`//@insert-line: <value>`

        :value: A line end terminated value.

    Example:
        :csharp:`//@insert-line: float getValue(); // declaration`

.. _conversion-rules-remove-line:

Remove Line
^^^^^^^^^^^
    Remove a redundant line, that is only needed in C# but not in GLSL.

    Rule:
        :csharp:`//@remove-line`

    Example:
        .. code-block:: csharp

            //@remove-line
            using System; // not needed in GLSL

.. _conversion-rules-text:

Text Rules
----------

Conversion rules that convert part of a line. These rules can be used with ``-all`` suffix (for example ``replace-all``) to make them run on all of the subsequent lines.

Replace / Remove Text
^^^^^^^^^^^^^^^^^^^^^
    Replace or remove all instances of a string.

    Rule:
        | :csharp:`//@replace, source: <instance>, target: <replacement>`
        | :csharp:`//@remove, source: <instance>, target: <replacement>`

        :source: the text to search (string, identifier, or a number).
        :target: the text to replace with (string, identifier, or a number).

Replace / Remove Word
^^^^^^^^^^^^^^^^^^^^^
    Replace or remove all instances of a word (surrounded by word boundaries).

    Rule:
        | :csharp:`//@replace-word, source: <word>, target: <replacement>`
        | :csharp:`//@remove-word, source: <word>, target: <replacement>`

        :source: the word to search (string, identifier, or a number).
        :target: the text to replace with (string, identifier, or a number).

Replace Regex
^^^^^^^^^^^^^
    Match regex, and replace using a pattern.

    Rule:
        :csharp:`//@replace-regex, source: <regex>, target: <replacement>`

        :source: Regex search pattern (string).
        :target: Replace pattern, using capture groups (string).

        Capture groups can be referenced with ``$`` and a group index, or a group name:

        - ``$1, $2, $3...``
        - ``$groupName``, or ``$(groupName)`` (defined with ``(?<groupName>...)``)

    Example:
        .. code-block:: csharp

            //@replace-regex, source: "(?<name>\\w+): (?<value>[0-9.]+)", target: "$name=$value"


.. _conversion-rules-language:

Language Specific Rules
-----------------------

Conversion rules that amend C# with GLSL specific syntax.

.. _conversion-rules-uniform:

Uniform
^^^^^^^
    Convert a field declaration into a uniform declaration, removing a :csharp:`const`, or a :csharp:`readonly` modifiers if used, and preserving the uniform line annotation properties.

    Rule:
        :csharp:`//@uniform` |br|
        :csharp:`//@uniform, key1: value1, key2: value2, ...`

    Example:
        Source C#

        .. code-block:: csharp

            //@uniform, min: 0.0, step: 0.1
            vec2 value = vec2(1.0, 2.0);

        Converted GLSL

        .. code-block:: glsl

            //@uniform, min: 0.0, step: 0.1
            uniform vec2 value = vec2(1.0, 2.0);


.. _conversion-rules-const:

Const
^^^^^
    Convert a variable declaration into a const value declaration.

    Rule:
        :csharp:`//@const`

    Example:
        Source C#

        .. code-block:: csharp

            //@const
            vec2 value = vec2(1.0, 2.0);

        Converted GLSL

        .. code-block:: glsl

            const vec2 value = vec2(1.0, 2.0);

.. _conversion-rules-define:

Define
^^^^^^
    Convert a variable declaration or a lambda method ("expression bodied function") into a :glsl:`#define` directive.

    Rule:
        :csharp:`//@define`

    Value Example:
        Source C#

        .. code-block:: csharp

            //@define
            float PI = 3.14159265f;

        Converted GLSL

        .. code-block:: glsl

            #define PI 3.14159265

    Macro Example:
        Source C#

        .. code-block:: csharp

            //@define
            float sqr(float x) => ((x) * (x));

        Converted GLSL

        .. code-block:: glsl

            #define sqr(x) ((x) * (x))

        .. note::

            Since the body of the method is used as it is, just like with a regular macro, parenthesis have to added around variables, and also parameters name should not conflict with accessed methods or members names, for example, the macro: |br| :glsl:`#define pow2(v, x) vec2(pow(v.x, x), pow(v.y, x))` would produce an invalid code at :glsl:`v.x`, as :glsl:`x` is ambiguous.
