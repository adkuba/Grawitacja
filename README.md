# Koncepcja gry
Musimy dolecec statkiem jak najdalej przy ograniczonym zasobie paliwa i przeszkodach w postaci planet.<br>
Planety maja grawitacje, statkiem bedzie mozna sterowac prawo lewo i do przodu. <br>
Paliwo bedzie mozna zatankowac przy niektorych losowych planetach. <br>

# TODO
- dopracowac system paliwa, mam juz wyliczanie ile zostalo zuzyte, liczba po prawej, dodac tankowanie przy niektorych losowych planetach, lepszy graficzny wskaznik zuzycia
- dodac ekran startowy z zapamietanym najlepszym wynikiem
- mam bardzo rzadko jakis blad czasami ze obiekt zostal juz usuniety a ja chce do niego uzyskac dostep
- dopracowac grafike, kontroler oswietlenia!, generowanie obiektow

# Do pomyslenia
## Wersja druga gry
Mozna dodac system zbierania monet, dzieki nim bedziemy mogli wynajmowac na dany lot lepsze statki, z wiekszym zbiornikiem paliwa, z wieksza mozliwoscia przyspieszenia, z wieksza max predkoscia. Pierwsza wersja gry jest ladna graficznie ale z prostym systemem rozgrywki. <br><br>

## Styl graficzny
Zrealizowac te punkty
- W tle grafika z gwiazdami, na ciemnym tle, skupiska gwiazd, mglawice, gwiazdy poruszaja sie tworzac efekt paralax (dodaje glebie), gwiazdy raczej w formie rombow z poswiata. Jak nocne niebo troche bardziej rysunkowy styl. JEDYNA TEKSTURA JAKA MUSZE STWORZYC WLASCIWIE
- Dwa typy planet - jedna jak sferyczne lustro z ciemnymi ksiezycami, druga z "mgla". Ksiezyce i mgla to granica dzialania grawitacji, lustro dodaje glebi, mgla klimatu.
- Od slonc w tle efekt lens flare, dodaje klimat
- Na biezacym widoku dodanie pojawiajacych sie malych swiecacych punktow rozmieszczonych roznie w 3d, czesc z nich moze sie poruszac jak komety, dodanie efektu glebi i klimat.
- Asteroidy lekko rozmyte bo sa najblizej kamery - efekt depth of field
- Menu glowne w tle ma przyciemniony blur sceny gry z jasnymi napisami

# Co zostalo juz zrobione
Najwazniejsze punkty w dzialaniu gry
- System jest oparty na dodawaniu force do playera, czy to od planet czy od sterowania.
- Gracz ma ograniczenie max predkosci, powyzej takiej predkosci zostaje ona wyciszana
- Dzialajaca sila na playera jest wyciszana zawsze
- Planety, asteroidy, slonca generuja sie w otoczce wokol kamery z losowa wielkoscia, od wielkosci zalezy sila grawitacji
- Ta liczba po lewej to wynik, jest zalezny od odleglosci od punktu startowego
- Sterowanie za pomoca strzalek