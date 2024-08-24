import os

# Configuration file for the Sphinx documentation builder.
#
# For the full list of built-in configuration values, see the documentation:
# https://www.sphinx-doc.org/en/master/usage/configuration.html

# -- Project information -----------------------------------------------------
# https://www.sphinx-doc.org/en/master/usage/configuration.html#project-information

project = 'Shaderlens'
copyright = '2024, Shaderlens'
author = 'Shaderlens'

# -- General configuration ---------------------------------------------------
# https://www.sphinx-doc.org/en/master/usage/configuration.html#general-configuration

extensions = [
    'sphinx_rtd_theme',
]

templates_path = ['_templates']
exclude_patterns = []

# -- Options for HTML output -------------------------------------------------
# https://www.sphinx-doc.org/en/master/usage/configuration.html#options-for-html-output

html_theme = "sphinx_rtd_theme"

# These folders are copied to the documentation's HTML output
html_static_path = ['_static']

html_logo = "images/logo512.png"
html_favicon = 'images/favicon.ico'

# These paths are either relative to html_static_path
# or fully qualified paths (eg. https://...)
html_css_files = [
    'css/custom.css',
]

html_copy_source = False
html_show_sourcelink = False

# https://sphinx-rtd-theme.readthedocs.io/en/stable/configuring.html
# Theme options
html_theme_options = {
    # Collapse navigation (False makes it tree-like)
    "collapse_navigation": True,
    # If True, the version number is shown at the top of the sidebar.
    "display_version": True
}

html_context = {}

if "DOCS_VERSION" in os.environ:
    html_context["version"] = os.environ.get("DOCS_VERSION")
else:
    html_context["version"] = "latest"

# https://pygments.org/styles/
import pygments.styles, pygments.token
pygments_style = "vs"

rst_prolog = """
.. role:: csharp(code)
   :language: csharp

.. role:: glsl(code)
   :language: glsl

.. role:: json(code)
   :language: json

.. |br| raw:: html

  <br/>
"""