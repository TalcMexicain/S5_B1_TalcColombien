# AudioJDR - README

## Description
**AudioJDR** est une application Windows conçue pour rendre les jeux de rôle textuels accessibles aux personnes déficientes visuelles. Grâce à la synthèse et à la reconnaissance vocale, les joueurs peuvent interagir avec le jeu uniquement par la voix, sans nécessiter d'interface graphique.

L'application se compose de deux parties principales :
- **Moteur de jeu** : Permet de jouer à des jeux de rôle audio interactifs.
- **Éditeur de jeu** : Facilite la création de jeux de rôle textuels accessibles.

## Fonctionnalités

### Partie Joueur :
- **Synthèse vocale** : Lecture des descriptions et des actions en jeu.
- **Reconnaissance vocale** : Interactions vocales pour progresser dans l'histoire.
- **Sauvegarde automatique** : Continuité assurée des parties en cours.
- **Support multilingue** : Expérience personnalisable en plusieurs langues.

### Partie Éditeur :
- **Création de jeux audio** : Génération d’histoires interactives à partir de textes.
- **Gestion intuitive** : Interface pour concevoir et modifier des histoires, événements et options.
- **Import/Export** : Compatibilité avec le moteur de jeu via un format JSON standard.
- **Gameplay non-linéaire** : Gestion avancée des événements et des choix.
- **Interface multilingue** : Accessibilité pour les créateurs du monde entier.

## Technologies Utilisées
- **.NET MAUI** : Framework multiplateforme.
- **C#** : Langage principal du développement.
- **Synthèse vocale Windows** : API pour la lecture audio des descriptions.
- **Reconnaissance vocale Windows** : API pour les interactions vocales.
- **JSON** : Format pour importer et exporter les jeux.

## Avancement

### Éditeur :
- Fonctionnalités terminées : Création, modification, import/export d’histoires avec événements et options.
- Interface entièrement responsive pour une utilisation fluide sur différentes tailles d’écran.

### Partie Joueur :
- Navigation et interaction vocales opérationnelles au sein d'une histoire.
- Expérience immersive et accessible pour les personnes déficientes visuelles.
- Certaines fonctionnalités secondaires en cours d'amélioration (comme la navigation hors-jeu).

## Prochaines Étapes
- **Amélioration de l’expérience utilisateur** pour les joueurs malentendants.
- **Nouvelles fonctionnalités** pour enrichir les histoires interactives.

## Installation

### Prérequis :
- **Système** : Windows 10 ou version ultérieure.
- **Environnement** : .NET MAUI installé.

### Instructions (avec la Release) :
Un tutoriel sur pdf est disponible -> [ici](./AudioJDRpdf)

### Instructions (avec Visual Studio 2022) :
1. Clonez ce dépôt GitHub :
   ```bash
   git clone https://github.com/dept-info-iut-dijon/S5_B1_TalcColombien.git
   ```
2. Ouvrez le projet dans **Visual Studio**.
3. Compilez et exécutez l’application.

## Contributeurs
- **Alexandre Guidet** *(Client)*
- **Florence Mendes** *(Chef de projet)*
- **Le Talc Colombien** *(Équipe de développement)*
