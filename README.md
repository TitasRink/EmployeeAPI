Task EmployeeAPI

Using the Inmemory Database, no need to download anything.


Dalinuosi grįžtamuoju ryšiu dėl atliktos užduoties: "kodas ganėtinai inconsistent (pvz. metodų pavadinimai). Netinkamai sumodeliuotas employee - boss ryšys, nenaudojami foreign keys. Employee bosas nurodomas kaip string property, kas gali būti betkokia reikšmė, nėra tikrinama ar toks bosas (employee) egzistuoja.
Employee paieška pagal bosą įgyvendinta neteisingai (Employee klasė paveldi iš CEO klasės, tai semantiškai reikštų, kad visi employee yra CEO, kas nėra tiesa). Metodų parametruose naudojamas FromQuery atributas, kuris šioje užduotyje netinkamas, Task.Run turėtų būti naudojamas CPU bound operacijoms, ne duombazės užklausoms. Ne visų asinchroninių metodų pavadinimai baigiasi su 'Async'.
Nereikėtų maišyti LINQ extension method ir LINQ query syntax. Duomenys neturėtų būti grąžinami suformatuoti tiesiog kaip tekstinė eilutė. Reikėtų visur naudoti tą patį formatą pvz. JSON. Inconsistent FromRoute atributų naudojimas, kai kur yra, kai kur nėra. Validacija turėtų būti atskira atsakomybė įgyvendinta savo klasėje, o ne išmėtyta per property set'erius. Employee validacija nepadengta testais."
