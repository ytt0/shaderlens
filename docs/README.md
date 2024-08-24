# Documentation

The documentation is built with [Sphinx](https://www.sphinx-doc.org/) using [Read the Docs](https://readthedocs.org/) theme.

The documentation should be updated and checked-in to the `main` branch regularly with new feature updates, the documentation is published when a new release is completed.

## Build the Documentation

### Prerequisites

1. Install [Python package installer](https://pip.pypa.io/en/stable/getting-started/) (pip)
2. Install the locked Sphinx packages
    ```
    pip install -r docs/requirements.txt
    ```
    - Alternatively Sphinx and Read the Docs packages could be installed directly:
        ```
        pip install sphinx
        pip install sphinx-rtd-theme
        ```
        (may not be compatible with the locked packages)

### Build

- Build with
  ```
  make html
  ```
    - Open `build\html\index.html`

- Clean with
  ```
  make clean
  ```
