﻿BEGIN TRAN

INSERT INTO Role
(ProjectUserId, ProjectId, Type)
VALUES
(3, 1, 2),
(1, 1, 1),
(2, 1, 1),
(4, 1, 1),
(1, 2, 2),
(1, 3, 1),
(1, 4, 1)

COMMIT