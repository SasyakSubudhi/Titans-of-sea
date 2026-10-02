from pathlib import Path
import json, re

root = Path(__file__).resolve().parents[1]
assets = root / 'Assets/_Project'
names = ['IOceanSurface','IWeatherService','ITimeOfDay','IPlayerInput','IInventory','IShipState','IUiNotifier','ICutsceneService','IAudioService','IVfxService','IQuestService']
for name in names:
    text = (assets / f'Scripts/Contracts/{name}.cs').read_text()
    assert f'interface {name}' in text, name
events = (assets / 'Scripts/Contracts/GameEvents.cs').read_text()
assert len(re.findall(r'public static event Action<', events)) == 25
assert len(re.findall(r'public static void Raise', events)) == 25
definitions = {}
for path in assets.rglob('*.asmdef'):
    data = json.loads(path.read_text())
    assert data['name'] not in definitions
    definitions[data['name']] = data
for name, data in definitions.items():
    assert all(ref in definitions for ref in data.get('references', [])), name
def visit(name, chain):
    assert name not in chain, 'Assembly cycle: ' + str(chain + [name])
    for ref in definitions[name].get('references', []): visit(ref, chain + [name])
for name in definitions: visit(name, [])
for path in assets.rglob('*.cs'):
    text = path.read_text()
    assert text.count('{') == text.count('}'), path
    assert '...' not in text, path
    assert 'UnityEditor' not in text or '/Editor/' in path.as_posix(), path
buoyancy = (assets / 'Scripts/Ship/ShipBuoyancy.cs').read_text().split('private void FixedUpdate()')[1].split('public void ResetPose')[0]
assert 'new ' not in buoyancy and 'Find(' not in buoyancy
print(f'PASS: {len(names)} interfaces, 25 event/raise pairs, {len(definitions)} acyclic assemblies, source structure and buoyancy allocation audit.')
