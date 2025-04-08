CREATE TABLE constellations (
    con_id serial primary key,
    con_name varchar(64) unique not null,
    iau_abbr char(3) unique not null check (regexp_like(iau_abbr, '^[a-zA-Z]{3}$')),
    nasa_abbr char(4) unique not null check (regexp_like(nasa_abbr, '^[a-zA-Z]{4}$')),
    genitive varchar(64) not null,
    origin varchar(255) not null,
    meaning varchar(64) not null
);

INSERT INTO constellations (con_name, iau_abbr, nasa_abbr, genitive, origin, meaning) VALUES
    ('Andromeda', 'And', 'Andr', 'Andromedae', 'ancient (Ptolemy)', 'Andromeda (The chained maiden or princess)'),
    ('Antlia', 'Ant', 'Antl', 'Antliae', '1763, Lacaille', 'air pump'),
    ('Apus', 'Aps', 'Apus', 'Apodis', '1603, Uranometria, created by Keyser and de Houtman', 'Bird-of-paradise/Exotic Bird/Extraordinary Bird'),
    ('Aquarius', 'Aqr', 'Aqar', 'Aquarii', 'ancient (Ptolemy)', 'water-bearer'),
    ('Aquila', 'Aql', 'Aqil', 'Aquilae', 'ancient (Ptolemy)', 'eagle'),
    ('Ara', 'Ara', 'Arae', 'Arae', 'ancient (Ptolemy)', 'altar'),
    ('Aries', 'Ari', 'Arie', 'Arietis', 'ancient (Ptolemy)', 'ram'),
    ('Auriga', 'Aur', 'Auri', 'Aurigae', 'ancient (Ptolemy)', 'charioteer'),
    ('Boötes', 'Boo', 'Boot', 'Boötis', 'ancient (Ptolemy)', 'herdsman'),
    ('Caelum', 'Cae', 'Cael', 'Caeli', '1763, Lacaille', 'chisel or engraving tool'),
    ('Camelopardalis', 'Cam', 'Caml', 'Camelopardalis', '1613, Plancius', 'giraffe'),
    ('Cancer', 'Cnc', 'Canc', 'Cancri', 'ancient (Ptolemy)', 'crab'),
    ('Canes Venatici', 'CVn', 'CVen', 'Canum Venaticorum', '1690, Firmamentum Sobiescianum, Hevelius', 'hunting dogs'),
    ('Canis Major', 'CMa', 'CMaj', 'Canis Majoris', 'ancient (Ptolemy)', 'greater dog'),
    ('Canis Minor', 'CMi', 'CMin', 'Canis Minoris', 'ancient (Ptolemy)', 'lesser dog'),
    ('Capricornus', 'Cap', 'Capr', 'Capricorni', 'ancient (Ptolemy)', 'sea goat'),
    ('Carina', 'Car', 'Cari', 'Carinae', '1763, Lacaille, split from Argo Navis', 'keel'),
    ('Cassiopeia', 'Cas', 'Cass', 'Cassiopeiae', 'ancient (Ptolemy)', 'Cassiopeia (mythological character)'),
    ('Centaurus', 'Cen', 'Cent', 'Centauri', 'ancient (Ptolemy)', 'centaur'),
    ('Cepheus', 'Cep', 'Ceph', 'Cephei', 'ancient (Ptolemy)', 'Cepheus (mythological character)'),
    ('Cetus', 'Cet', 'Ceti', 'Ceti', 'ancient (Ptolemy)', 'sea monster (later interpreted as a whale)'),
    ('Chamaeleon', 'Cha', 'Cham', 'Chamaeleontis', '1603, Uranometria, created by Keyser and de Houtman', 'chameleon'),
    ('Circinus', 'Cir', 'Circ', 'Circini', '1763, Lacaille', 'compasses'),
    ('Columba', 'Col', 'Colm', 'Columbae', '1592, Plancius, split from Canis Major', 'dove'),
    ('Coma Berenices', 'Com', 'Coma', 'Comae Berenices', '1536, Caspar Vopel, split from Leo', 'Berenice''s hair'),
    ('Corona Australis', 'CrA', 'CorA', 'Coronae Australis', 'ancient (Ptolemy)', 'southern crown'),
    ('Corona Borealis', 'CrB', 'CorB', 'Coronae Borealis', 'ancient (Ptolemy)', 'northern crown'),
    ('Corvus', 'Crv', 'Corv', 'Corvi', 'ancient (Ptolemy)', 'crow'),
    ('Crater', 'Crt', 'Crat', 'Crateris', 'ancient (Ptolemy)', 'cup'),
    ('Crux', 'Cru', 'Cruc', 'Crucis', '1589, Plancius, split from Centaurus', 'southern cross'),
    ('Cygnus', 'Cyg', 'Cygn', 'Cygni', 'ancient (Ptolemy)', 'swan or Northern Cross'),
    ('Delphinus', 'Del', 'Dlph', 'Delphini', 'ancient (Ptolemy)', 'dolphin'),
    ('Dorado', 'Dor', 'Dora', 'Doradus', '1603, Uranometria, created by Keyser and de Houtman', 'dolphinfish'),
    ('Draco', 'Dra', 'Drac', 'Draconis', 'ancient (Ptolemy)', 'dragon'),
    ('Equuleus', 'Equ', 'Equl', 'Equulei', 'ancient (Ptolemy)', 'pony'),
    ('Eridanus', 'Eri', 'Erid', 'Eridani', 'ancient (Ptolemy)', 'river Eridanus (mythology)'),
    ('Fornax', 'For', 'Forn', 'Fornacis', '1763, Lacaille', 'chemical furnace'),
    ('Gemini', 'Gem', 'Gemi', 'Geminorum', 'ancient (Ptolemy)', 'twins'),
    ('Grus', 'Gru', 'Grus', 'Gruis', '1603, Uranometria, created by Keyser and de Houtman', 'crane'),
    ('Hercules', 'Her', 'Herc', 'Herculis', 'ancient (Ptolemy)', 'Hercules (mythological character)'),
    ('Horologium', 'Hor', 'Horo', 'Horologii', '1763, Lacaille', 'pendulum clock'),
    ('Hydra', 'Hya', 'Hyda', 'Hydrae', 'ancient (Ptolemy)', 'Hydra (mythological creature)'),
    ('Hydrus', 'Hyi', 'Hydi', 'Hydri', '1603, Uranometria, created by Keyser and de Houtman', 'lesser water snake'),
    ('Indus', 'Ind', 'Indi', 'Indi', '1603, Uranometria, created by Keyser and de Houtman', 'Indian (of unspecified type)'),
    ('Lacerta', 'Lac', 'Lacr', 'Lacertae', '1690, Firmamentum Sobiescianum, Hevelius', 'lizard'),
    ('Leo', 'Leo', 'Leon', 'Leonis', 'ancient (Ptolemy)', 'lion'),
    ('Leo Minor', 'LMi', 'LMin', 'Leonis Minoris', '1690, Firmamentum Sobiescianum, Hevelius', 'lesser lion'),
    ('Lepus', 'Lep', 'Leps', 'Leporis', 'ancient (Ptolemy)', 'hare'),
    ('Libra', 'Lib', 'Libr', 'Librae', 'ancient (Ptolemy)', 'balance'),
    ('Lupus', 'Lup', 'Lupi', 'Lupi', 'ancient (Ptolemy)', 'wolf'),
    ('Lynx', 'Lyn', 'Lync', 'Lyncis', '1690, Firmamentum Sobiescianum, Hevelius', 'lynx'),
    ('Lyra', 'Lyr', 'Lyra', 'Lyrae', 'ancient (Ptolemy)', 'lyre / harp'),
    ('Mensa', 'Men', 'Mens', 'Mensae', '1763, Lacaille, as Mons Mensæ', 'Table Mountain (South Africa)'),
    ('Microscopium', 'Mic', 'Micr', 'Microscopiae', '1763, Lacaille', 'microscope'),
    ('Monoceros', 'Mon', 'Mono', 'Monocerotis', '1613, Plancius', 'unicorn'),
    ('Musca', 'Mus', 'Musc', 'Muscae', '1603, Uranometria, created by Keyser and de Houtman', 'fly'),
    ('Norma', 'Nor', 'Norm', 'Normae', '1763, Lacaille', 'carpenter''s level'),
    ('Octans', 'Oct', 'Octn', 'Octantis', '1763, Lacaille', 'octant (instrument)'),
    ('Ophiuchus', 'Oph', 'Ophi', 'Ophiuchi', 'ancient (Ptolemy)', 'serpent-bearer'),
    ('Orion', 'Ori', 'Orio', 'Orionis', 'ancient (Ptolemy)', 'Orion (mythological character)'),
    ('Pavo', 'Pav', 'Pavo', 'Pavonis', '1603, Uranometria, created by Keyser and de Houtman', 'peacock'),
    ('Pegasus', 'Peg', 'Pegs', 'Pegasi', 'ancient (Ptolemy)', 'Pegasus (mythological winged horse)'),
    ('Perseus', 'Per', 'Pers', 'Persei', 'ancient (Ptolemy)', 'Perseus (mythological character)'),
    ('Phoenix', 'Phe', 'Phoe', 'Phoenicis', '1603, Uranometria, created by Keyser and de Houtman', 'phoenix'),
    ('Pictor', 'Pic', 'Pict', 'Pictoris', '1763, Lacaille, as Equuleus Pictoris', 'easel'),
    ('Pisces', 'Psc', 'Pisc', 'Piscium', 'ancient (Ptolemy)', 'fishes'),
    ('Piscis Austrinus', 'PsA', 'PscA', 'Piscis Austrini', 'ancient (Ptolemy)', 'southern fish'),
    ('Puppis', 'Pup', 'Pupp', 'Puppis', '1763, Lacaille, split from Argo Navis', 'stern deck'),
    ('Pyxis', 'Pyx', 'Pyxi', 'Pyxidis', '1763, Lacaille', 'mariner''s compass'),
    ('Reticulum', 'Ret', 'Reti', 'Reticuli', '1763, Lacaille', 'eyepiece graticule'),
    ('Sagitta', 'Sge', 'Sgte', 'Sagittae', 'ancient (Ptolemy)', 'arrow'),
    ('Sagittarius', 'Sgr', 'Sgtr', 'Sagittarii', 'ancient (Ptolemy)', 'archer'),
    ('Scorpius', 'Sco', 'Scor', 'Scorpii', 'ancient (Ptolemy)', 'scorpion'),
    ('Sculptor', 'Scl', 'Scul', 'Sculptoris', '1763, Lacaille', 'sculptor'),
    ('Scutum', 'Sct', 'Scut', 'Scuti', '1690, Firmamentum Sobiescianum, Hevelius', 'shield (of Sobieski)'),
    ('Serpens', 'Ser', 'Serp', 'Serpentis', 'ancient (Ptolemy)', 'snake'),
    ('Sextans', 'Sex', 'Sext', 'Sextantis', '1690, Firmamentum Sobiescianum, Hevelius', 'sextant'),
    ('Taurus', 'Tau', 'Taur', 'Tauri', 'ancient (Ptolemy)', 'bull'),
    ('Telescopium', 'Tel', 'Tele', 'Telescopii', '1763, Lacaille', 'telescope'),
    ('Triangulum', 'Tri', 'Tria', 'Trianguli', 'ancient (Ptolemy)', 'triangle'),
    ('Triangulum Australe', 'TrA', 'TrAu', 'Trianguli ', '1603, Uranometria, created by Keyser and de Houtman', 'southern triangle'),
    ('Tucana', 'Tuc', 'Tucn', 'Tucanae', '1603, Uranometria, created by Keyser and de Houtman', 'toucan'),
    ('Ursa Major', 'UMa', 'UMaj', 'Ursae Majoris', 'ancient (Ptolemy)', 'great bear'),
    ('Ursa Minor', 'UMi', 'UMin', 'Ursae Minoris', 'ancient (Ptolemy)', 'lesser bear'),
    ('Vela', 'Vel', 'Velr', 'Velorum', '1763, Lacaille, split from Argo Navis', 'sails'),
    ('Virgo', 'Vir', 'Virg', 'Virginis', 'ancient (Ptolemy)', 'virgin or maiden'),
    ('Volans', 'Vol', 'Voln', 'Volantis', '1603, Uranometria, created by Keyser and de Houtman, as Piscis Volans', 'flying fish'),
    ('Vulpecula', 'Vul', 'Vulp', 'Vulpeculae', '1690, Firmamentum Sobiescianum, Hevelius, as  Vulpecula cum Ansere', 'fox');
