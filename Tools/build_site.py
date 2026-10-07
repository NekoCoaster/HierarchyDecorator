"""Render the standard VCC listing UI without modifying release packages."""
import argparse
import html
import json
import pathlib
import shutil

ROOT = pathlib.Path(__file__).resolve().parents[1]

def version_key(version):
    core, _, pre = version.split("+", 1)[0].partition("-")
    return tuple(int(x) for x in core.split(".")) + (not pre, tuple((0, int(x)) if x.isdigit() else (1, x) for x in pre.split(".")))

def render(template, values):
    for key, value in values.items():
        template = template.replace("@@" + key + "@@", html.escape(str(value), quote=True))
    return template

def build_site(listing, output):
    output.mkdir(parents=True, exist_ok=True)
    source = ROOT / "Website"
    packages = {}
    rows = []
    for name, package in listing["packages"].items():
        versions = package["versions"]
        if not versions:
            continue
        stable = [v for v in versions if "-" not in v.split("+", 1)[0]]
        latest = max(stable or list(versions), key=version_key)
        info = dict(versions[latest])
        info.setdefault("licensesUrl", "https://github.com/NekoCoaster/HierarchyDecorator/blob/master/LICENSE.md")
        packages[name] = info
        rows.append(render((source / "row.html").read_text(encoding="utf-8"), {
            "DISPLAYNAME": info.get("displayName", name), "NAME": name,
            "DESCRIPTION": info.get("description", ""), "TYPE": "Any",
            "ZIPURL": info["url"]}))
    page = render((source / "index.html").read_text(encoding="utf-8"), {
        "NAME": listing["name"], "DESCRIPTION": "Unity hierarchy tools for VRChat avatars and worlds.",
        "URL": listing["url"], "AUTHOR_NAME": "NekoCoaster", "AUTHOR_URL": "https://github.com/NekoCoaster"})
    page = page.replace("@@ROWS@@", "\n".join(rows))
    if "@@" in page or "{{" in page:
        raise ValueError("Unresolved page template token")
    (output / "index.html").write_text(page, encoding="utf-8")
    (output / "index.json").write_text(json.dumps(listing, indent=2) + "\n", encoding="utf-8")
    (output / "packages.json").write_text(json.dumps(packages, indent=2) + "\n", encoding="utf-8")
    for name in ("app.js", "styles.css", "TEMPLATE-NOTICE.txt"):
        shutil.copyfile(source / name, output / name)

if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("--listing", required=True, type=pathlib.Path)
    parser.add_argument("--output", required=True, type=pathlib.Path)
    args = parser.parse_args()
    build_site(json.loads(args.listing.read_text(encoding="utf-8")), args.output)
