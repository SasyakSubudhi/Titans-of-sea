# Shared repository setup

Public repository: https://github.com/SasyakSubudhi/Titans-of-sea

The Unity project is the repository root. Both teammates must use **Unity 6.3 LTS 6000.3.25f1**. Clone this project instead of creating separate projects to merge.

## Branches

- `main`: tested baseline.
- `dev`: shared integration branch.
- `b/foundation-buoyancy`: Person B's initial gameplay work.
- `a/<feature>`: Person A's presentation work.

Open pull requests into dev. Review and integration testing precede merging. Move a tested dev milestone into main. Agree on editing windows before changing Main_World; use separate sandboxes meanwhile.

## Teammate setup

1. Install Git and Git LFS, then run `git lfs install`.
2. Clone `https://github.com/SasyakSubudhi/Titans-of-sea.git`.
3. In Unity Hub, add the cloned folder and open it with 6000.3.25f1.
4. Open `Assets/_Project/Scenes/B_Sandbox_Buoyancy.unity` and press Play.
5. Configure your own Git author name and email before making personal commits. Repository-local configuration is sufficient.

The repository includes Assets and their .meta files, Packages, ProjectSettings, docs and tools. Library, Temp, Logs, Obj, UserSettings and Builds are excluded. Existing LFS rules cover future art/audio assets; install LFS before adding them. Preserve .meta files across Windows and Mac.

The initial foundation commit is authored by `Codex <codex@local.invalid>`. Subsequent commits in this checkout use Sasyak Subudhi's GitHub noreply address. No collaborator invitation has been sent; the owner can invite Person A through Settings > Collaborators when ready.
