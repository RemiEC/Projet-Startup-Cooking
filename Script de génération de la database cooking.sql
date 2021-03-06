CREATE database cooking;
use cooking;
CREATE TABLE IF NOT EXISTS `cooking`.`Client` (
  `Identifiant` VARCHAR(50) NOT NULL,
  `Mot_de_passe` VARCHAR(50) NOT NULL,
  `Nom_Client` VARCHAR(50) NULL,
  `Numero_tel_Client` VARCHAR(15) NULL,
  `Credit_Cook` INT NULL,
  `CdR` INT NULL, 
  PRIMARY KEY (`Identifiant`) );
 
CREATE TABLE IF NOT EXISTS `cooking`.`Commande` (
  `Ref_Commande` INT NOT NULL AUTO_INCREMENT,
  `Date` DATE NOT NULL,
  `prix` INT NULL,
  `Identifiant` VARCHAR(20) NOT NULL,
  PRIMARY KEY (`Ref_Commande`),
  CONSTRAINT `Identifiant_Commande` FOREIGN KEY (`Identifiant`)
		REFERENCES `cooking`.`client` (`Identifiant`)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION );

CREATE TABLE IF NOT EXISTS `cooking`.`Recette` (
  `Nom_Recette` VARCHAR(100) NOT NULL,
  `Type` VARCHAR(30) NULL,
  `Descriptif` VARCHAR(256) NULL,
  `Prix_Vente` INT NULL,
  `Remuneration` INT NULL,
  `Compteur` INT NULL,
  `Identifiant` VARCHAR(20) NOT NULL,
  PRIMARY KEY (`Nom_Recette`),
  CONSTRAINT `Identifiant_Recette` FOREIGN KEY (`Identifiant`)
		REFERENCES `cooking`.`client` (`Identifiant`)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION );

CREATE TABLE IF NOT EXISTS `cooking`.`Composition_Commande` (
  `Nom_Recette` VARCHAR(30) NOT NULL,
  `Ref_Commande` INT NOT NULL AUTO_INCREMENT,
  `Quantite_Recette` INT NOT NULL,
  PRIMARY KEY (`Nom_Recette`, `Ref_Commande`),
   CONSTRAINT `Nom_Recette_Composition_Commande` FOREIGN KEY (`Nom_Recette`)
		REFERENCES `cooking`.`Recette` (`Nom_Recette`)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION,
   CONSTRAINT `Ref_Commande_Composition_Commande` FOREIGN KEY (`Ref_Commande`)
		REFERENCES `cooking`.`Commande` (`Ref_Commande`)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION  );

CREATE TABLE IF NOT EXISTS `cooking`.`Fournisseur` (
  `Ref_Fournisseur` VARCHAR(50) NOT NULL,
  `Nom_Fournisseur` VARCHAR(50) NOT NULL,
  `Numero_tel_Fournisseur` VARCHAR(15) NULL,
  PRIMARY KEY (`Ref_Fournisseur`) );

CREATE TABLE IF NOT EXISTS `cooking`.`Produit` (
  `Nom_Produit` VARCHAR(50) NOT NULL,
  `Categorie` VARCHAR(50) NOT NULL,
  `Unite` VARCHAR(10) NOT NULL,
  `Stock` INT NULL,
  `Stock_min` INT NULL,
  `Stock_max` INT NULL,
  `Ref_Fournisseur` VARCHAR(50) NOT NULL,
  PRIMARY KEY (`Nom_Produit`),
   CONSTRAINT `Ref_Fournisseur_Produit` FOREIGN KEY (`Ref_Fournisseur`)
		REFERENCES `cooking`.`Fournisseur` (`Ref_Fournisseur`)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION );

CREATE TABLE IF NOT EXISTS `cooking`.`Composition_Recette` (
  `Nom_Recette` VARCHAR(30) NOT NULL,
  `Nom_Produit` VARCHAR(30) NOT NULL,
  `Quantite_Produit` INT NOT NULL,
  PRIMARY KEY (`Nom_Recette`, `Nom_Produit`),
   CONSTRAINT `Nom_Recette_Composition_Recette` FOREIGN KEY (`Nom_Recette`)
		REFERENCES `cooking`.`Recette` (`Nom_Recette`)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION,
   CONSTRAINT `Nom_Produit_Composition_Recette` FOREIGN KEY (`Nom_Produit`)
		REFERENCES `cooking`.`Produit` (`Nom_Produit`)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION  );

