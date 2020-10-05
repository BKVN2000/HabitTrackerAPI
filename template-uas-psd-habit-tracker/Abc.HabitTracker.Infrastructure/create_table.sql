CREATE TABLE "User"
(
    id        	UUID PRIMARY KEY,
    name       	VARCHAR(100) NOT NULL
);

CREATE TABLE "Habit"
(
    id          UUID PRIMARY KEY,
    name  		VARCHAR(200) NOT NULL,
    created_at  TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
   	user_id     UUID NOT NULL, 		
	day_off		TEXT[],
    deleted_at  TIMESTAMPTZ
	FOREIGN KEY (user_id) REFERENCES "User" (id)
);

CREATE TABLE "log_habit"
(
    id         		UUID PRIMARY KEY,
    user_id       	UUID NOT NULL,
    habit_id 		UUID NOT NULL,
    created_at 		TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
	isDayOff		boolean,
	FOREIGN KEY (user_id)  REFERENCES "User" (id),
	FOREIGN KEY (habit_id) REFERENCES "Habit"(id)
);

CREATE TABLE "Badge"
(
	id         		UUID PRIMARY KEY,
	name			VARCHAR(100),
	description		VARCHAR(100),
	created_at 		TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE "BadgeAchievement"
(
    user_id 		UUID NOT NULL,
	badge_id		UUID NOT NULL,
    created_at 		TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
	FOREIGN KEY 	(user_id) REFERENCES "User" (id),
	FOREIGN KEY 	(badge_id) REFERENCES "Badge" (id),
	PRIMARY KEY		(user_id,badge_id)
); 

  CREATE TABLE "streak"(
  		id	UUID PRIMARY KEY,
	  	user_id UUID NOT NULL,
	  	habit_id UUID NOT NULL,
	  	streak INT DEFAULT 2,
	  	created_at  TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
	  	last_update_at TIMESTAMPTZ,
        last_log_habit_id UUID,
        FOREIGN KEY (last_log_habit_id) REFERENCES "log_habit" (id),
	  	FOREIGN KEY (user_id) REFERENCES "User" (id), 
    	FOREIGN KEY (habit_id) REFERENCES "Habit" (id) 
  )

--insert query for badge
INSERT INTO "Badge" VALUES (uuid_generate_v1(), 'Dominating', '4+streak')
INSERT INTO "Badge" VALUES (uuid_generate_v1(), 'Workaholic' ,'Doing someworks on daysoff')
INSERT INTO "Badge" VALUES (uuid_generate_v1(),'Epic Comeback','10 streak after 10 days without logging')



				