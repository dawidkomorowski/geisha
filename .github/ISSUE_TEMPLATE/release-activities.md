---
name: Release activities
about: Gather all required tasks related to releasing each new version.
title: Release Geisha SDK <version>
labels: ''
assignees: dawidkomorowski

---

Tasks required to release new version:
- [ ] Update copyright in `csproj` files
- [ ] Update copyright in `LICENSE` file
- [ ] Review `sdk\readme.txt` file
- [ ] Review `src\Geisha.Cli\ThirdPartyNotices.txt` file
- [ ] Download RC of `GeishaSDK.zip` and review its content
- [ ] Publish GitHub release
  - [ ] Tag commit with version in format `vMajor.Minor.Patch`
  - [ ] Prepare release notes
  - [ ] Attach `GeishaSDK.zip`
- [ ] Publish documentation
- [ ] Update `project.version`
