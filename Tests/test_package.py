import hashlib
import importlib.util
import json
import pathlib
import tempfile
import unittest
import zipfile

ROOT = pathlib.Path(__file__).resolve().parents[1]
spec = importlib.util.spec_from_file_location("build_vpm", ROOT / "Tools/build_vpm.py")
builder = importlib.util.module_from_spec(spec)
spec.loader.exec_module(builder)

class PackageTests(unittest.TestCase):
    def test_installable_archive_and_listing(self):
        with tempfile.TemporaryDirectory() as directory:
            output = pathlib.Path(directory)
            archive = builder.build(output)
            listing = json.loads((output / "index.json").read_text())
            with zipfile.ZipFile(archive) as package:
                manifest = json.loads(package.read("package.json"))
                self.assertEqual(manifest["displayName"], "HierarchyDecorator - Neko's Fork")
                self.assertIn("LICENSE.md", package.namelist())
                self.assertFalse(any(n.startswith((".git", "Tests/", "Tools/")) for n in package.namelist()))
                for name in package.namelist():
                    if name.endswith(".cs"):
                        self.assertIn(name + ".meta", package.namelist())
            entry = listing["packages"][manifest["name"]]["versions"][manifest["version"]]
            self.assertEqual(entry["zipSHA256"], hashlib.sha256(archive.read_bytes()).hexdigest())
            self.assertTrue(entry["url"].endswith('/v' + manifest['version'] + '/' + archive.name))
            self.assertEqual(manifest['legacyPackages'], ['com.wooshii.hierarchydecorator'])
            original = archive.read_bytes()
            versions = listing['packages'][manifest['name']]['versions']
            versions['0.12.0'] = {'version': '0.12.0'}
            prior = output / 'prior.json'
            prior.write_text(json.dumps(listing))
            builder.build(output, prior)
            self.assertEqual(original, archive.read_bytes())
            self.assertIn('0.12.0', json.loads((output / 'index.json').read_text())['packages'][manifest['name']]['versions'])

if __name__ == '__main__':
    unittest.main()
