# TDR-Simulacio (25.03.2022)
Primera versió de la simulació. Inclou:

## Menús
**Menú principal**

- Presenta un botó per preparar la simulació i un per sortir.

- Si s'apreta (P) abans de fer clic a res, igual passi alguna cosa graciosa...
  
**Menú de preparació de la simulació**
  
- Es poden veure en una llista totes les espècies de dos tipus: predeterminades i personalitzades. Les espècies predeterminades actuals són gallines i guineus. Hi ha un botó amb el qual es poden crear espècies personalitzades.

- Cada element de la llista té un nombre a l'esquerra que es pot modificar amb un botó a sobre seu que l'augmenta en 1, i amb un botó a sota seu que el disminueix en 1. Aquest és el nombre d'individus d'aquesta espècie amb què iniciarà la simulació.

- Si es fa clic a una espècie de les llistes s'accedeix als seus paràmetres d'espècie, que es poden modificar només en les espècies personalitzades. Aquests paràmetres son:

  - Modificables i visibles en el menú:
    
    - Nom en singular i en plural, i el gènere del nom de l'espècie (_La_ gallina, _El_ lleó).

    - Velocitat al caminar i córrer: base i variació.

    - Temps d'afamació, assedegament, ganes de reproducció i gestació: base i variació.

    - Detecció entorn: base i variació.

    - Fills màxims i mínims en cada embaràs.
      
  - No modificables i no visibles en el menú (Ho seran en el futur):
    
    - Dieta primària, secundària i terciària.

    - Depredadors.

    - Temps desenvolupament cries: base i variació.

    - Desenvolupament cries: variació.

    - Grups: sí o no, i en cas de sí, individus màxims per grup.   
      
- Finalment, hi ha un botó per iniciar la simulació.

## La simulació
**Escenari**
  
- La simulació consta d'un senzill i visualment desagradable escenari: un pla amb un riu. En el futur es millorarà.
  
- Hi ha uns cubs de color verd fosc que representen plantes i van apareixent més amb el temps.
  
**Càmera (Vista actual de l'escenari)**
  
- Es pot moure la càmera cap endavant o endarrere amb la roda del ratolí.
  
- Si es manté premut clic dret i es mou el ratolí, la càmera gira.
  
- Si es manté premuda la roda del ratolí i es mou aquest, la càmera es desplaça horitzontal i verticalment.
  
**Interfície**
  
- La interfície consta de 3 menús que es poden desplegar i replegar: el menú del temps, el menú d'individu i el menú d'espècie.
  
  - **Menú del temps**
    
    - Un text a la zona superior del menú mostra el temps que ha passat en la simulació.
    
    - A sota apareix un botó de pausa, amb el qual la simulació es pot pausar. També es pot pausar si s'apreta (ESPAI)
    
    - Al costat apareixen 5 botons de velocitat, que permeten que el temps avanci més ràpidament o lentament. També es pot canviar la velocitat amb les tecles (1), (2), (3), (4) i (5).
    
  - **Menú d'individu**
    
    - Si no s'ha seleccionat cap individu, en aquest menú només apareixerà un text que indica que no hi ha cap individu seleccionat. Per seleccionar un individu, cal fer-hi clic.
    
    - Un cop s'hagi seleccionat un individu, apareixerà un text que indica de quina espècie es tracta, el seu estat actual, la seva gana, set i ànsia reproductiva actual i tots els seus gens, incloent el seu gènere.
    
    - També apareixerà un botó amb el qual es pot deseleccionar l'individu o refrescar la informació. La informació no es refresca sola.
    
  - **Menú d'espècie**

    - En aquest menú apareixerà una llista de totes les espècies, tant predeterminades com personalitzades, independentment de si es troben o no a la simulació actual.
    
    - Si es fa clic a una espècie, apareixerà la seva població actual, un gràfic població-temps i un gràfic amb les mitjanes dels gens en tots els individus respecte el temps. La informació es recopila cada 30 segons. 
    
    - També apareix un botó per refrescar els gràfics. Els gràfics no es refresquen sols. El botó de refrescar NO fa que es recopilin més dades, només fa que es mostrin les dades que no apareixen als gràfics actualment.
    
**Individus**
- A l'inici de la simulació, apareixen tots els individus prèviament determinats en el menú de preparació de la simulació. La meitat dels individus de cada espècie seran mascles, i l'altra meitat, femelles.
  
- Un cop tots els individus han aparegut, la simulació comença.
  
- El comportament bàsic dels individus és el següent:
  
  - Per defecte, trien un punt a l'atzar per caminar-hi cada cert temps.
  - Si tenen gana, trien un punt a l'atzar per caminar-hi cada poc temps fins que veuen menjar, llavors hi van. Un cop arriben, se'l mengen.
  - (La set no està implementada encara)
  - Si tenen ganes de reproduir-se, trien un punt a l'atzar per caminar-hi cada poc temps fins que veuen un individu de l'altre gènere que també busqui parella, llavors hi van. Si es troben dos, copulen i la femella s'embarassa, i finalment té fills.
    
**Gens**
- Els individus tenen un genoma, que és una llista de gens. Totes les espècies tenen els mateixos tipus de gens. Cada gen és, essencialment, un nombre entre -1 i 1. A l'inici de la simulació tots els gens de tots els individus són 0.
  
- Els gens actuals són (hi haurà més en el futur):
  - Velocitat: Moure's més ràpid a canvi de consumir més energia. (L'energia no està implementada encara.)
  - Detecció: Detectar millor l'entorn. (El gen existeix però no fa res encara.)
  - Ànsia reproductiva: El temps que triga a voler reproduir-se.
  - Color:  El color de l'individu (El gen existeix però no fa res encara.)
  - Atractiu: Gen exclusiu masculí. Probabilitat de ser acceptat per una femella per reproduir-se. (El gen existeix però no fa res encara.)
  - Gestació: Gen exclusiu femení. Temps de gestació. Si és més curt, els descendents estaran menys desenvolupats. (El desenvolupament dels descendents no està implementat encara.)
- Quan dos individus es reprodueixen, cada fill, per cada un dels gens dels progenitors:
  - Si el gen és exclusiu masculí, el gen serà el mateix que el pare.
  - Si el gen és exclusiu femení, el gen serà el mateix que la mare.
  - Si el gen no és exclusiu de cap gènere, hi ha un 45% de probabilitat que sigui el del pare; un 45% de probabilitat que sigui el de la mare i un 10% de probabilitat que sigui la mitjana dels dos gens.
    
  - A part de tot això, hi ha un 10% de probabilitat que el gen muti, aleshores s'hi suma un nombre a l'atzar entre -0,5 i 0,5.
    
- Per determinar, per exemple, la velocitat de caminar d'un individu, es pren la seva velocitat de caminar base i variació dels seus paràmetres d'espècie i el seu gen de velocitat. Imaginem que són 5, 3 i 0,6 respectivament. Llavors, la velocitat de l'individu seria _base + gen · variació_ , per tant, 5 + 3 · 0,6 = 6,8. Això s'aplica a tots els gens per determinar diferents paràmetres que fan que cada individu sigui diferent.
  
  
Això és tot per aquesta primera versió. Encara queda molta feina, i pràcticament res d'això està acabat.

## Coses que falten

- Els animals no busquen ni beuen aigua.

- Els animals no fugen de depredadors.

- Els animals no es desenvolupen amb el temps ni els afecta l'edat.

- Els animals no canvien de color.

- Els animals morts no desapareixen.

- Els animals no estan animats.

- Més espècies.

- Més gens.

- Afegir maneres de destacar i apropar la càmera a l'individu seleccionat. Costa molt saber quin és un cop perdut de vista.

- Poder sortir d'una simulació i tornar al menú principal.

- Millorar els menús i visuals en general.

- Ordenar el codi.

- Optimització. Molta optimizació.

- Etc.

## Errors coneguts

- El programa es confon quan es crea més d'una espècie personalitzada.

- El botó de refrescar gràfics no fa res.

- A vegades els individus es registren com a morts més d'un cop, de manera que el nombre de la població disminueix massa, fins i tot podent arribar a ser nombres negatius.

- Els botons de pausar i de velocitat a vegades no s'actualitzen correctament quan es pausa la simulació.
