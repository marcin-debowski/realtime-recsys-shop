import importlib

import pytest


@pytest.mark.parametrize("package", ["service", "training", "simulator"])
def test_package_is_importable(package: str) -> None:
    assert importlib.import_module(package).__doc__
