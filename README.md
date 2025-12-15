# H1

## Pitch - Hvad har i lavet og hvilke teknologier har i brugt (blazor + den database i bruger) - Maks 150 ord

Dette er et blazor Projekt samt en postgress database.
Vi har lavet en markedsplads, hvor man kan købe og sælge brugte puslespil. Vi har har lavet vores styling med css fra bunden, så den skulle ligne et billiard bord. 
I vores project kan man oprette brugere og annoncer, som kan redigeres af deres ejer.

## Embedded Video og ~~designfil~~ 



## Opstartsguide af projektet - Hvordan starter man det!

For at starte projektet skal du som minimum have installeret .NET CLI eller .NET
https://learn.microsoft.com/en-us/dotnet/core/install/windows

du skal også bruge git
https://git-scm.com/install/windows



Åben søg i windows og søg på "cmd" og tryk enter
kør kommandoerne

```
cd desktop

git clone https://github.com/Mercantec-GHC/projekt-puzzles.git

```

i "projekt-puzzles/Blazor/Data" er der en fil der hedder "ConnectionString-example.cs" fjern alle `//` for at gør brug af koden og indsæt et connection string til en postgress database

her efter fortsæt med disse kommandoer

```
cd projekt-puzzles/Blazor

dotnet run
```