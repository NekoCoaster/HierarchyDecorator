import { baseLayerLuminance, StandardLuminance } from 'https://unpkg.com/@fluentui/web-components@2.6.1';
await Promise.all(['fluent-dialog', 'fluent-button', 'fluent-text-field'].map(name => customElements.whenDefined(name)));
baseLayerLuminance.setValueFor(document.documentElement, StandardLuminance.DarkMode);
const byId = id => document.getElementById(id);
const listingUrl = byId('vccUrlField').value;
const status = byId('status');
const help = byId('addListingToVccHelp');
const details = byId('packageInfoModal');
const menu = byId('rowMoreMenu');
const add = () => window.location.assign(`vcc://vpm/addRepo?url=${encodeURIComponent(listingUrl)}`);
byId('vccAddRepoButton').addEventListener('click', add);
document.querySelectorAll('.rowAddToVccButton').forEach(button => button.addEventListener('click', add));
byId('urlBarHelp').addEventListener('click', () => { help.hidden = false; });
byId('packageInfoListingHelp').addEventListener('click', () => { help.hidden = false; });
byId('addListingToVccHelpClose').addEventListener('click', () => { help.hidden = true; });
byId('packageInfoModalClose').addEventListener('click', () => { details.hidden = true; });
document.addEventListener('keydown', event => {
  if (event.key === 'Escape') { help.hidden = true; details.hidden = true; menu.hidden = true; }
});
for (const [buttonId, fieldId] of [['vccUrlFieldCopy','vccUrlField'], ['vccListingInfoUrlFieldCopy','vccListingInfoUrlField'], ['packageInfoVccUrlFieldCopy','packageInfoVccUrlField']]) {
  byId(buttonId).addEventListener('click', async () => {
    const field = byId(fieldId);
    try { await navigator.clipboard.writeText(field.value); status.textContent = 'Repository URL copied.'; }
    catch { field.focus(); field.select(); status.textContent = 'Select and copy the repository URL above.'; }
  });
}
byId('searchInput').addEventListener('input', event => {
  const query = event.target.value.toLowerCase().trim();
  let shown = 0;
  document.querySelectorAll('#packageGrid fluent-data-grid-row[row-type="default"]').forEach(row => {
    const matches = (row.dataset.packageName + ' ' + row.dataset.packageId).toLowerCase().includes(query);
    row.classList.toggle('hidden', !matches);
    if (matches) shown++;
  });
  status.textContent = shown ? '' : 'No matching packages.';
});
let downloadUrl;
document.querySelectorAll('.rowMenuButton').forEach(button => button.addEventListener('click', event => {
  event.stopPropagation();
  downloadUrl = event.currentTarget.dataset.packageUrl;
  const rect = event.currentTarget.getBoundingClientRect();
  menu.style.position = 'fixed';
  menu.style.top = `${rect.bottom}px`;
  menu.style.left = `${Math.max(8, rect.right - 160)}px`;
  menu.hidden = false;
}));
byId('rowMoreMenuDownload').addEventListener('click', () => {
  if (downloadUrl?.startsWith('https://')) window.open(downloadUrl, '_blank', 'noopener');
  menu.hidden = true;
});
document.addEventListener('click', event => { if (!menu.contains(event.target)) menu.hidden = true; });
let packages;
try {
  const response = await fetch('packages.json');
  if (!response.ok) throw new Error('Package details unavailable');
  packages = await response.json();
} catch { status.textContent = 'Package details could not load. Add to VCC is still available.'; }
document.querySelectorAll('.rowPackageInfoButton').forEach(button => button.addEventListener('click', event => {
  const id = event.currentTarget.dataset.packageId;
  const info = packages?.[id];
  if (!info) { status.textContent = 'Package details unavailable. Please reload the page.'; return; }
  byId('packageInfoName').textContent = info.displayName || id;
  byId('packageInfoId').textContent = id;
  byId('packageInfoVersion').textContent = `v${info.version}`;
  byId('packageInfoDescription').textContent = info.description || '';
  byId('packageInfoAuthor').textContent = info.author?.name || 'Not specified';
  byId('packageInfoAuthor').href = info.author?.url?.startsWith('https://') ? info.author.url : '#';
  byId('packageInfoLicense').textContent = info.license || 'See license';
  byId('packageInfoLicense').href = info.licensesUrl?.startsWith('https://') ? info.licensesUrl : '#';
  const dependencies = byId('packageInfoDependencies'); dependencies.replaceChildren();
  const entries = Object.entries({...info.dependencies, ...info.vpmDependencies});
  for (const text of entries.length ? entries.map(([name, version]) => `${name} ${version}`) : ['None']) {
    const li = document.createElement('li'); li.textContent = text; dependencies.append(li);
  }
  const keywords = byId('packageInfoKeywords'); keywords.replaceChildren();
  for (const word of info.keywords || []) { const span = document.createElement('span'); span.className = 'badge me-2'; span.textContent = word; keywords.append(span); }
  details.hidden = false;
}));
