# Koncepcja gry
Musimy dolecec statkiem jak najdalej przy ograniczonym zasobie paliwa i przeszkodach.<br>
Planety maja grawitacje, statkiem bedzie mozna sterowac prawo lewo i do przodu. <br>
Paliwo bedzie mozna zatankowac przy niektorych planetach. <br>

# TODO
- dopracowac system paliwa, mam juz wyliczanie ile zostalo zuzyte, liczba po prawej, dodac tankowanie przy niektorych planetach, lepszy wskaznik zuzycia
- dodac ekran startowy z zapamietanym najlepszym wynikiem
- utworzyc losowe dodatkowe przeszkody bez pola grawitacji aby rozgrywka byla trudniejsza
- mam bardzo rzadko jakis blad czasami ze obiekt zostal juz usuniety a ja chce do niego uzyskac dostep

# Do pomyslenia
Mozna dodac system zbierania monet, dzieki nim bedziemy mogli wynajmowac na dany lot lepsze statki, z wiekszym zbiornikiem paliwa, z wieksza mozliwoscia przyspieszenia, z wieksza max predkoscia <br><br>

Styl graficzny bardziej taki rysunkowy, planety na ktorych mozna zatankowac moga miec jakies postacie na niej, tam gdzie nie mozemy tankowac beda jakies proste niezamieszkale planety. Zamiast planet moze byc slonce. Player to jakas postac, statek moze miec np jakies cechy ozywione xd kurwa.

# Co zostalo juz zrobione
Najwazniejsze punkty <br>
- System jest oparty na dodawaniu force do playera, czy to od planet czy od sterowania.
- Gracz ma ograniczenie max predkosci, powyzej takiej predkosci zostaje ona wyciszana
- Dzialajaca sila na playera jest wyciszana zawsze
- Planety generuja sie w otoczce wokol kamery z losowa wielkoscia, od wielkosci zalezy sila grawitacji
- Ta liczba po lewej to wynik, jest zalezny od odleglosci od punktu startowego
- Sterowanie za pomoca strzalek

# Opis commitow
- <b>first:</b> generowanie obiektow ok, bez losowej wielkosci
- <b>przyrost2:</b> dodana losowosc wielkosci, prototyp wygladu pola grawitacyjnego
- <b>przyrost3:</b> dodana otoczka grawitacji, tło, zainstalowalem pro builder tam zrobic model 3d gracza, dodana grawitacja
- <b>przyrost4:</b> pomysł ze słońcem, zmiana koncepcji gry asysta grawitacyjna jest niemozliwa bo planety musza sie poruszac, zostawic grawitacje tak jak teraz jest, paliwo zbieramy na zamieszkanych planetach, pozniej pojawiaja sie przeszkody (moze asteroidy), musimy doleciec jak najdalej nadal, dopracowany generator, dodana kolizja, poczatek systemu paliwo, wyciszanie pradkosci tylko powyzej jakiegos poziomu!!!!
- <b>przyrost5:</b> podwojne tło, porzadki

# TODOs done
- OK dodac kolizje i paliwo
- OK oswietlenie planety musi zalezec od jej wielkosci! dostosowac to w kodzie, dlaczego sa takie bugi w oswietleniu - musialem ustawic swiatla na important
- OK dopracować generator, wieksze planety i wiekszy obszar generowania
- OK slonce to spot light nad planeta
- OK dodac losowosc wielkosci planet!!!
- OK wskaznik gdzie jest najblizsza planeta NIE oddalilem troche widok i jest ok
- OK dodac grawitacje, w player controller dodac wyliczenie