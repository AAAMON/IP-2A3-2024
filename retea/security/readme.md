Exemplul din cod e destul de self explanatory.

Folosim RSA, securitate cu o pereche de chei, una publica si alta privata.

Toate aceste chestii sunt menajate de un obiect de tipul System.Security.Cryptography.RSA

Sa spunem ca A vrea sa trimita mesaj criptat la B.

1. A cere/primeste cheia publica de la B.
2. A cripteaza mesajul folosind aceasta cheie.
3. A trimite mesajul criptat lui B
4. B decripteaza mesajul folosind cheia sa privata.

Atentie, cu propria sa cheie privata, cineva poate decripta doara mesaje criptate cu cheia sa publica.

Similar, daca B vrea sa trimita un mesaj criptat lui A, care sa-l poata decripta, B cere mai intai cheia publica de la A...