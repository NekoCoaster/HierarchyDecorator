import copy
import json
import pathlib
import tempfile
import unittest

from Tools.build_site import build_site


class SiteTests(unittest.TestCase):
    def test_preserves_releases_and_escapes_package_metadata(self):
        versions = {}
        for version in ('1.9.0', '1.10.0', '2.0.0-beta.1'):
            versions[version] = {
                'version': version,
                'displayName': 'Example <tool> & friends',
                'description': 'A "quoted" description',
                'url': 'https://example.com/' + version + '.zip',
                'zipSHA256': 'unchanged-release-checksum',
            }
        listing = {'name': 'Test listing', 'url': 'https://example.com/index.json',
                   'packages': {'test.package': {'versions': versions}}}
        original = copy.deepcopy(listing)
        with tempfile.TemporaryDirectory() as directory:
            output = pathlib.Path(directory)
            build_site(listing, output)
            self.assertEqual(listing, original)
            self.assertEqual(json.loads((output / 'index.json').read_text()), original)
            details = json.loads((output / 'packages.json').read_text())
            self.assertEqual(details['test.package']['version'], '1.10.0')
            page = (output / 'index.html').read_text(encoding='utf-8')
            self.assertIn('Example &lt;tool&gt; &amp; friends', page)
            self.assertNotIn('@@', page)
            self.assertIn('https://example.com/1.10.0.zip', page)
            for asset in ('app.js', 'styles.css', 'TEMPLATE-NOTICE.txt'):
                self.assertTrue((output / asset).is_file())
