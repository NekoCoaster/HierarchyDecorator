"""Build a VPM ZIP and repository listing using only the Python standard library."""
import argparse
import hashlib
import json
import pathlib
import re
import zipfile
try:
    from Tools.build_site import build_site
except ModuleNotFoundError:
    from build_site import build_site

ROOT = pathlib.Path(__file__).resolve().parents[1]

def build(output, previous=None):
    manifest = json.loads((ROOT / "package.json").read_text(encoding="utf-8"))
    name, version = manifest["name"], manifest["version"]
    if not re.fullmatch(r"[0-9]+\.[0-9]+\.[0-9]+(?:-[a-zA-Z0-9.-]+)?", version):
        raise ValueError("Invalid package version")
    output.mkdir(parents=True, exist_ok=True)
    archive = output / f"{name}-{version}.zip"
    paths = list((ROOT / "HierarchyDecorator").rglob("*"))
    paths += [ROOT / n for n in ("HierarchyDecorator.meta", "package.json", "package.json.meta", "README.md", "README.md.meta", "LICENSE.md", "LICENSE.md.meta", "CHANGELOG.md", "CHANGELOG.md.meta")]
    with zipfile.ZipFile(archive, "w", zipfile.ZIP_DEFLATED) as package:
        for path in sorted(paths):
            if path.is_file():
                info = zipfile.ZipInfo(path.relative_to(ROOT).as_posix(), (2026, 1, 1, 0, 0, 0))
                info.compress_type = zipfile.ZIP_DEFLATED
                package.writestr(info, path.read_bytes())
    listing = json.loads(previous.read_text()) if previous and previous.exists() else {
        "name": "HierarchyDecorator - Neko's Fork",
        "id": "com.nekocoaster.hierarchydecorator.repository",
        "url": "https://nekocoaster.github.io/HierarchyDecorator/index.json",
        "author": "NekoCoaster", "packages": {}}
    entry = dict(manifest, zipSHA256=hashlib.sha256(archive.read_bytes()).hexdigest())
    listing["packages"].setdefault(name, {"versions": {}})["versions"][version] = entry
    (output / "index.json").write_text(json.dumps(listing, indent=2) + "\n", encoding="utf-8")
    build_site(listing, output)
    return archive

if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("--output", type=pathlib.Path, default=ROOT / "dist")
    parser.add_argument("--previous", type=pathlib.Path)
    args = parser.parse_args()
    print(build(args.output, args.previous))
