# WorkoutXpCalculatorService detail design

Az osztály  5 darab metódust tartalmaz:
- CalculateWorkoutXp
- LimitWorkoutXp
- CalculateStrengthXp
- CalculateBodyweightXp
- CalculateCardioXp

Az osztály lényege, hogy egy WorkoutSession objektumra kiszámolja, hogy összesen mennyi XP-t ér tehetségi szintenként.
Ezt úgy teszi meg, hogy a CalculateWorkoutXp függvény megkap egy WorkoutSession objektumot, amiben egy lista SessionExercise van,
a SessionExercise-ok nak van egy típusa, lehetnek vagy Strength vagy Cardio vagy Bodyweight gyakorlatok.

## CalculateWorkoutXp
`Bemenet`: egy WorkoutSession objektum, amelyben a gyakorlatok listája található.

A metódustól azt várjuk, hogy végigmenjen ezen a listán, és minden gyakorlatot a saját ExerciseType mezője alapján dolgozzon fel. 
Ha a gyakorlat erő típusú, akkor az erőhöz tartozó számítási szabályt kell alkalmaznia. (CalculateStrengthXp)
Ha kardió típusú, akkor az állóképességhez tartozó szabályt kell használnia. (CalculateCardioXp)
Ha saját testsúlyos gyakorlatról van szó, akkor a testkontroll kategóriába kell számolnia. (CalculateBodyweightXp)
A három kategória számítása egymástól független, tehát az egyik kategória eredménye semmilyen módon nem módosíthatja a másik kettőt.
Az algoritmus végén, mindegyik kategória XP-je korlátozva lesz MaxExerciseXp konstans értékre, hogy ne lehessen XP-t farmolni.

`Kimenet`: XpResult osztály, amely három egész számot tartalmaz a thetségi szintek xp-jéhez: StrengthXp, EnduranceXp és BodyControlXp. Ezek az szintek az összes adott típusú gyakorlatra kiszámolt xp összegével egyenlőek.


### LimitWorkoutXp
`Bemenet`: egy lebegőpontos szám, amely az adott kategória összesített XP-jét jelenti. 
Ellenőrizi, hogy az érték meghaladja-e a MaxExerciseXp konstans értékét. Ha igen, akkor a visszatérési érték MaxExerciseXp lesz, különben a bemeneti értéket adja vissza. A kimenet mindig egész számra kerekített érték lesz.

`Kimenet`: egész szám

### CalculateStrengthXp
`Bemenet`: egy SessionExercise objektum aminek az exerciseType mezője strength
Kontextus: a strength típusú gyakolratoknak 2 mezőjével számolunk: Weight és Reps, 
Ezen két szám szorzata a volumen, ami egy gyök alá kerül, hogy ne legyen lineáris az xp számítás.

`Kimenet`: egy lebegőpontos szám ami limitálva van a MaxExcerciseXp-re.
Az algoritmus lényege, hogy nagyobb súly és sok ismétlést jutalmazzuk.

### CalculateCardioXp
`Bemenet`: egy SessionExercise objektum aminek az exerciseType mezője cardio

`Kontextus`: a cardio típusú gyakolratoknak 2 mezőjével számolunk: Duration(min) és Distance(meter)

A függvény elösször átváltja a percet órára 60-al osztással, majd kiszámítja a sebességet km/h-val.
Az alap XP értéke kilóméterentként ér CardioDistanceMultiplier-et.
Az intenzitás szorzó, ami minimum a CardioIntesityMin vagy maximum a CardioIntensityMax konstansok közé esik. Amúgy meg, ha egyiket se éri el akkor a CardioBaseIntensity szorozva a sebesség szorozva a CardioSpeedIntensityFactor konstassal.

`Kimenet`: A végleges XP a intensitás szorzó lesz az alapxp-vel megszorozva, ez egy lebegőpontos szám ami limitálva van a MaxExcerciseXp-re.
Az algoritmus lényege itt, hogy azt a gyakorlatot jutalmazzuk, ahol gyorsabban több távot tud megtenni a felhasználó.

### CalculateBodyweightXp
`Bemenet`: egy SessionExercise objektum aminek az exerciseType mezője bodyweight

`Kontextus`: a bodyweight típusú gyakolratoknak 1 mezőjével számolunk: reps 

A függvény az XP-t úgy számítja, hogy a reps-eket felemeli BodyweightExponent konstans értékre, majd ezt megszorozza BodyweightBaseMultiplier-el. Így itt sem lineáris az XP nyereség.

`Kimenet`: egy lebegőpontos szám ami limitálva van a MaxExcerciseXp-re.

### Mindegyik XP számolos függvényre igaz
Elösször megnézi, hogy a kontextus mező null-e, ha igen nullára állítja. Ha pedig bármelyik kontextus mező kisebb mint 0 akkor 0-át ad vissza a függvény.

### Hibás működés
A CalculateWorkoutXp nem kaphat null WorkoutSession-t, ha létező WorkoutSession-t kap null SessionExercise listával, akkor az összes XP érték 0 lesz.

### További kontextus:
A TestHelpers osztályt használd a tesztadatok előállítására.

## Elvárt bemenetek és kimenetek

#### Vegyes edzés
Egy vegyes edzés esetén azt várjuk, hogy a CalculateWorkoutXp metódus minden gyakorlatot a saját típusának megfelelően dolgozzon fel, majd a három kategóriára külön-külön összegezze az XP-t. 
Ha az edzésben van egy erő gyakorlat, egy kardió gyakorlat és egy saját testsúlyos gyakorlat, akkor mindhárom kategória kapjon értéket, és a visszatérési értékben ezek egymástól függetlenül jelenjenek meg.

#### Egy típusú edzés
Ha az edzésben csak érvényes erő gyakorlatok szerepelnek, akkor csak a StrengthXp érték legyen nagyobb nullánál, az EnduranceXp és a BodyControlXp pedig maradjon nulla. 
Ugyanez igaz a másik két gyakorlattípusra is: csak az a kategória növekedjen, amelyikhez a gyakorlat tartozik.

#### Hibás vagy hiányos adatok
Ha egy gyakorlat adatai hiányosak vagy hibásak, például a súly nulla, az ismétlésszám nulla, az időtartam hiányzik vagy a távolság negatív, akkor az adott gyakorlat ne adjon XP-t. 
Ilyenkor a metódusnak nem szabad hibával leállnia, hanem az adott elem hozzájárulása nulla legyen.

#### Összegzés és korlátozás
Ha az edzésben több azonos típusú gyakorlat szerepel, akkor ezek XP-jét össze kell adni. 
Például két erő gyakorlat esetén a két részérték összege jelenjen meg a StrengthXp mezőben, majd ez az érték legyen korlátozva a MaxExerciseXp határra.

#### Kerekítés és egész számok
Ha egy kategória összesített XP-je meghaladná a MaxExerciseXp értékét, akkor a visszaadott eredményben már nem jelenhet meg ennél nagyobb szám. 
Az eredménynek minden esetben egész számnak kell lennie, és a kerekítés a .NET alapértelmezett szabálya szerint történik.

#### Üres edzés vagy null WorkoutSession
Ha a WorkoutSession-ben az Exercises lista üres, akkor a metódusnak mindhárom kategóriára 0-t kell visszaadnia. 
Ha maga a WorkoutSession null, akkor a jelenlegi működés alapján futásidejű hiba várható, ezért ezt a bemenetet a rendszer nem támogatott esetként kezeli.