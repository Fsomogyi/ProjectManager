﻿BEGIN TRAN

INSERT INTO Role
(ProjectUserId, ProjectId, Type)
VALUES
(1, 1, 2),
(1, 2, 1),
(1, 3, 1),
(1, 4, 1),
(2, 1, 1),
(2, 2, 2),
(4, 1, 1),
(4, 4, 2),
(3, 3, 2)

COMMIT