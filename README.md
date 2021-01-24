# DDRSnapshot
A mobile app that keeps track of one's scores in the game Dance Dance Revolution. Features the ability to take a picture and have the score be automatically saved without manual user input. Also provides information not given by the machine itself to help decide which song to play next, whether one has a certain pattern, etc. Uses Xamarin Forms to write a shared codebase between Android and iOS. UI is in XAML. Uses an MVVC design. Uses Realm as the database on the phone, and uses an ODM to communicate between the MongoDB database in the cloud.

# TODO List

- [x] Hook up to Online Database
- [x] Implement Offline Database
- [x] Implement Camera page (Pick/Take pictures)
- [x] Process picture to model
- [x] Setup initial data
- [x] Implement Search Functionality to SongList
- [ ] Implement Filters to SongListPage
- [ ] Implement Options/Settings Page
- [ ] Implement Stats Screen
- [ ] Song info - (BPM, Patterns(crossovers, ))

## For Later
- Add more proper fields to model (jacket, perfect, marvelous, combo, etc.)
- Make this app look prettier (customize ui)
- Train my own number recognition system
