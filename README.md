# Spis treści
- [Koncepcja gry](#koncepcja-gry)
- [Funkcje](#funkcje)   
   - [Generator obiektów](#generator-obiektów)   
   - [Sterowanie](#sterowanie)   
   - [Skalowanie trudności](#skalowanie-trudności)   
   - [Obiekty](#obiekty)   
   - [Inne](#inne)
- [Wartości](#wartości)
- [Druga wersja gry](#druga-wersja-gry)

# Koncepcja gry
Musimy dolecieć statkiem jak najdalej przy ograniczonym zasobie paliwa i przeszkodach w postaci planet oraz asteroid.<br>
Planety mają grawitację, statkiem bedzie mozna sterować prawo lewo, dodatkowo co jakis czas będzie dostępne przyśpieszenie.<br>
Paliwo tankuje się przy niektórych, losowych planetach.

# Funkcje
## Generator obiektów
Obiekty w grze powstają losowo. Generator wywołuje co określony czas 3 funkcje tworzące planety, asteroidy lub słońca. Podczas generowania wybierany jest losowo obszar w którym funkcja będzie próbowała maksymalnie 10 razy znaleźć odpowiednią pozycję obiektu. Obszary to: nad graczem, na prawo, na lewo lub na dole. Pozycja wybierana jest w obszarze losowo, a następnie sprawdzane jest czy są zachowane odpowiednie odległości od innych obiektów. Jeśli nie uda się znaleźć odpowiedniej pozycji w 10 próbach to obiekt nie powstaje. Potencjalne usprawnienie procesu to zastąpienie w jakiś spsób tych 10 prób. Może się zdarzyć że w żadnej próbie nie trafimy w odpowiednią pozycję i gra się "zapycha".

## Sterowanie
Gdy klikamy w lewą połowę ekranu skręcamy w lewo, gdy w prawą to w prawo, kliknięcie na raz prawej i lewej części aktywuje przyśpiesznie. Podczas zwolnienia w systemie pomagającym uniknąć kolizji siła skrętu jest większa. Jeśli nie orbitujemy przez określony czas, paliwo zużywa się szybciej. Przyśpieszenia nie możemy użyć orbitując, mniej niż 2s po opuszczeniu orbity lub gdy nie mamy wystarczającej ilości paliwa. Używając przyśpieszenia dostajemy bonus do wyniku od 0 do 100 punktów.<br><br>
W grze zdefiniowana jest maksymalna prędkość gracza, powyżej tej wartości ruch jest wyciszany. Analogicznie jest z minimalną prędkością gracza. Po wylocie z orbity planety, jeśli orbitujemy określony czas, dostajemy dodatkowe przyśpieszenie. Gdy skończy nam się paliwo ruch jest wyciszany po czym pojawia się opcja wyświetlenia reklamy i kontynuowania gry. Siła stosowana na statek jest podczs każdej klatki wyciszana.

## Skalowanie trudności
Gra do poziomu 200 punktów (odległość) jest prosta, a przy 1600 punktów osiąga największą trudność.
- Gdy gracz nie klika w ekran przez określony czas (zmniejsza się wraz ze wzrostem odległości) przed graczem powstają asteroidy.
- Prędkość gry (timeScale) oraz ilość możliwych asteroid rośnie wraz ze wzrostem odległości.
- Kąt w systemie umożliwiającym ominięcie kolizji (zwolnienie), dąży do zera wraz ze wzrostem odległości. 

## Obiekty
Planety mają 1/3 prawdopodobieństwa że możemy zatankować orbitując je. Najbardziej prawdopodobną konfiguracją jest planeta z iskrami, lub planeta z księżycem. Rzadziej spotykana to iskry oraz książyc i planeta bez dodatkowych obiektów. Kierunek obrotu książyca jest wybierany losowo. <br><br>
Planety mają losową wiekość z podanego zakresu, od ich wielkości zależy siła przyciągania. Obiekty usuwają się automatycznie po przekroczeniu zdefiniowanej granicy (inna dla każdego obiektu). Uwaga słońce jest najbardziej kosztowne w renderowaniu (dodatkwe oświetlenie).

## Inne
- Gdy w menu wejdziemy i wyjdziemy z info w sumie 20 razy, pojawia się easterEgg
- Tło menu to gra bez statku kosmicznego, sterujemy za pomocą akcelerometru.

# Wartości
- Wskaźnik paliwa: kolor 864564  (RGB)
- Wskaźnik przyspieszenia (kremowe koło): kolor f1d1c4 (RGB)
- Wskaźnik odległości: czcionka OCR A Extended

# Druga wersja gry
Można dodać system zbierania monet, dzieki nim będziemy mogli wynajmować na dany lot lepsze statki, z większym zbiornikiem paliwa, z wiekszą możliwością przyśpieszenia, z większą max predkością.