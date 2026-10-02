# Shared repository setup — after the first sandbox works

No GitHub repository has been created or published by this kit.

Local Git is now initialized in TitansUnity on `b/foundation-buoyancy`, with the tested foundation committed. Git LFS rules are ready for future assets. The initial commit uses `Codex <codex@local.invalid>` because no user identity was configured. Set your own repository-local Git name/email before your first personal commit. The remaining steps below describe connecting to GitHub and establishing main/dev after teammate review.

1. Both teammates use Unity 6.3 LTS 6000.3.25f1. One teammate creates the Universal 3D project and imports the B kit; the other clones this same project rather than creating a second project to merge.
2. In GitHub, create a private repository named `titans-of-the-sea` and invite the teammate under repository Settings → Collaborators. Keep the online repository empty if publishing an existing local project.
3. Install Git and Git LFS on both machines. In the actual Unity project folder, initialize Git, use `main` as the default branch, and run `git lfs install`.
4. Copy the source kit's .gitignore into the project root before staging files. Configure LFS tracking for large art/audio files such as .fbx, .blend, .psd, .wav and .mp4 before adding them. Commit .gitattributes too. Keep source, scene/prefab YAML and .meta files in ordinary Git.
5. Commit Assets, Packages, ProjectSettings and docs. Exclude Library, Temp, Logs, Obj, UserSettings and Builds. Publish to the private repository with GitHub Desktop or Git.
6. Create `dev` from the working `main` baseline. B works on `b/foundation-buoyancy`; A uses `a/<feature>`. Open pull requests into dev; teammate review and integration tests precede merge. Merge a tested dev milestone into main.
7. Both edit separate sandboxes. Agree before editing Main_World; one editor at a time. Include every new Unity .meta file so object identities stay stable across Windows and Mac.

Suggested first B commit: `feat(b): add buoyancy, clock and weather foundation`.
