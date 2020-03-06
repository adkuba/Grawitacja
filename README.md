# Koncepcja gry
Musimy dolecec statkiem jak najdalej przy ograniczonym zasobie paliwa i przeszkodach w postaci planet.<br>
Planety maja grawitacje, statkiem bedzie mozna sterowac prawo lewo.<br>
Paliwo bedzie mozna zatankowac przy niektorych losowych planetach. <br>

# Do pomyslenia
## Wersja druga gry
Mozna dodac system zbierania monet, dzieki nim bedziemy mogli wynajmowac na dany lot lepsze statki, z wiekszym zbiornikiem paliwa, z wieksza mozliwoscia przyspieszenia, z wieksza max predkoscia. Pierwsza wersja gry jest ladna graficznie ale z prostym systemem rozgrywki. <br><br>

## Styl graficzny
Zrealizowac te punkty
- W tle grafika z gwiazdami, na ciemnym tle, skupiska gwiazd, mglawice, gwiazdy poruszaja sie tworzac efekt paralax (dodaje glebie).
- Typy planet - odbice, ksiezyce, iskry. Ksiezyce i iskry to granica dzialania grawitacji, lustro dodaje glebi, iskry klimatu.
- Od slonc w tle efekt lens flare, dodaje klimat
- Asteroidy lekko rozmyte bo sa najblizej kamery - efekt depth of field
- Menu glowne w tle ma przyciemniony blur sceny gry z jasnymi napisami

# Co zostalo juz zrobione
Najwazniejsze punkty w dzialaniu gry
- System jest oparty na dodawaniu force do playera, czy to od planet czy od sterowania.
- Gracz ma stala predkosc, powyzej niej zwalnia automatycznie, ponizej przyspiesza
- Planety, asteroidy, slonca generuja sie w otoczce wokol kamery z losowa wielkoscia, od wielkosci zalezy sila grawitacji
- Sterowanie za pomoca strzalek, dodawanie sily zaleznej od czasu! z czasem jest coraz silniej im dluzej trzymam