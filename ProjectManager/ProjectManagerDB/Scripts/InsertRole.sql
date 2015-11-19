BEGIN TRAN

INSERT INTO Role
(UserId, ProjectId, Type)
VALUES
(3, 1, 2),
(1, 1, 1),
(2, 1, 1),
(4, 1, 1)

COMMIT