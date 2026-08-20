create table if not exists music (
    id uuid primary key,
    name varchar(160) not null,
    artist varchar(160) not null,
    duration_seconds integer not null check (duration_seconds > 0),
    bpm integer null check (bpm > 0),
    original_key varchar(16) not null
);

create table if not exists setlist (
    id uuid primary key,
    name varchar(160) not null,
    status varchar(24) not null
);

create table if not exists setlist_song (
    setlist_id uuid not null references setlist(id),
    music_id uuid not null references music(id),
    position integer not null check (position > 0),
    performance_key varchar(16) not null,
    primary key (setlist_id, music_id),
    unique (setlist_id, position)
);