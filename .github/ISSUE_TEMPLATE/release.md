---
name: Release
about: Check-list of all actions required to release a new version.
title: Release Geisha SDK <version>
labels: ''
assignees: dawidkomorowski

---

Actions required to release a new version:
- [ ] Update copyright
  - [ ] Update copyright in `LICENSE` file
  - [ ] Update copyright in `Directory.Build.props`
- [ ] Review documents
  - [ ] Review `sdk\readme.txt` file
  - [ ] Review `src\Geisha.Cli\ThirdPartyNotices.txt` file
  - [ ] Update SDK version in documentation website
  - [ ] Review documentation website
  - [ ] Review GitHub `README.md`
- [ ] Download Release Candidate of `GeishaSDK.zip` and review its content
- [ ] Publish documentation
- [ ] Publish GitHub release
  - [ ] Tag commit with version in format `vMajor.Minor.Patch`
  - [ ] Prepare release notes
  - [ ] Attach `GeishaSDK.zip`
- [ ] Update `VersionPrefix` and `PreviousVersionTag` in `Directory.Build.props`
